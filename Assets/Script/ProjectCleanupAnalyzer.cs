using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

public class ProjectCleanupAnalyzer : MonoBehaviour
{
    [Header("Project Cleanup Analysis")]
    [SerializeField] private bool includeDetailedReport = true;
    [SerializeField] private bool analyzeScenes = true;
    [SerializeField] private bool analyzePrefabs = true;
    [SerializeField] private bool analyzeScripts = true;
    [SerializeField] private bool analyzeAssets = true;
    
    private HashSet<string> usedFiles = new HashSet<string>();
    private HashSet<string> allProjectFiles = new HashSet<string>();
    private Dictionary<string, List<string>> fileDependencies = new Dictionary<string, List<string>>();
    private List<string> protectedFiles = new List<string>();
    private List<string> unusedFiles = new List<string>();
    
    [ContextMenu("Analyze Project Files")]
    public void AnalyzeProjectFiles()
    {
        #if UNITY_EDITOR
        Debug.Log("Starting comprehensive project file analysis...");
        
        // Clear previous analysis
        usedFiles.Clear();
        allProjectFiles.Clear();
        fileDependencies.Clear();
        protectedFiles.Clear();
        unusedFiles.Clear();
        
        // Step 1: Get all project files
        CollectAllProjectFiles();
        
        // Step 2: Identify protected files (Unity system files, packages, etc.)
        IdentifyProtectedFiles();
        
        // Step 3: Analyze file usage
        if (analyzeScenes) AnalyzeSceneUsage();
        if (analyzePrefabs) AnalyzePrefabUsage();
        if (analyzeScripts) AnalyzeScriptUsage();
        if (analyzeAssets) AnalyzeAssetUsage();
        
        // Step 4: Check build settings and project settings
        AnalyzeBuildSettings();
        AnalyzeProjectSettings();
        
        // Step 5: Generate unused files list
        GenerateUnusedFilesList();
        
        // Step 6: Create report
        CreateAnalysisReport();
        
        Debug.Log("Project analysis complete!");
        #else
        Debug.LogWarning("This tool only works in Unity Editor!");
        #endif
    }
    
    #if UNITY_EDITOR
    private void CollectAllProjectFiles()
    {
        Debug.Log("Collecting all project files...");
        
        string[] assetPaths = AssetDatabase.GetAllAssetPaths();
        foreach (string path in assetPaths)
        {
            if (path.StartsWith("Assets/") || path.StartsWith("Packages/"))
            {
                allProjectFiles.Add(path);
            }
        }
        
        Debug.Log($"Found {allProjectFiles.Count} total files in project");
    }
    
    private void IdentifyProtectedFiles()
    {
        Debug.Log("Identifying protected files...");
        
        foreach (string file in allProjectFiles)
        {
            // Protect Unity package files
            if (file.StartsWith("Packages/"))
            {
                protectedFiles.Add(file);
                continue;
            }
            
            // Protect Unity-generated files
            if (file.Contains("/.") || file.EndsWith(".meta"))
            {
                protectedFiles.Add(file);
                continue;
            }
            
            // Protect essential project files
            string fileName = Path.GetFileName(file);
            if (fileName.StartsWith("ProjectSettings") || 
                fileName.StartsWith("UserSettings") ||
                fileName.Contains("csc.rsp") ||
                fileName.Contains("mcs.rsp") ||
                fileName.Contains(".asmdef") ||
                fileName.Contains("Assembly-"))
            {
                protectedFiles.Add(file);
                continue;
            }
            
            // Protect currently open scenes
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!string.IsNullOrEmpty(scene.path) && scene.path == file)
                {
                    protectedFiles.Add(file);
                    break;
                }
            }
        }
        
        Debug.Log($"Identified {protectedFiles.Count} protected files");
    }
    
    private void AnalyzeSceneUsage()
    {
        Debug.Log("Analyzing scene usage...");
        
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);
            usedFiles.Add(scenePath);
            
            // Analyze scene contents
            AnalyzeSceneFile(scenePath);
        }
        
        // Also check build settings scenes
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        foreach (var scene in buildScenes)
        {
            if (!string.IsNullOrEmpty(scene.path))
            {
                usedFiles.Add(scene.path);
                AnalyzeSceneFile(scene.path);
            }
        }
    }
    
    private void AnalyzeSceneFile(string scenePath)
    {
        try
        {
            string sceneContent = File.ReadAllText(scenePath);
            
            // Find script references
            MatchCollection scriptMatches = Regex.Matches(sceneContent, @"m_Script: \{fileID: \d+, guid: ([a-f0-9]+)");
            foreach (Match match in scriptMatches)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(scriptPath))
                {
                    usedFiles.Add(scriptPath);
                    AddFileDependency(scenePath, scriptPath);
                }
            }
            
            // Find material references
            MatchCollection materialMatches = Regex.Matches(sceneContent, @"guid: ([a-f0-9]+).*?type: 2");
            foreach (Match match in materialMatches)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    usedFiles.Add(assetPath);
                    AddFileDependency(scenePath, assetPath);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not analyze scene {scenePath}: {e.Message}");
        }
    }
    
    private void AnalyzePrefabUsage()
    {
        Debug.Log("Analyzing prefab usage...");
        
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in prefabGuids)
        {
            string prefabPath = AssetDatabase.GUIDToAssetPath(guid);
            
            // Check if prefab is referenced by scenes or other prefabs
            if (IsAssetReferenced(prefabPath))
            {
                usedFiles.Add(prefabPath);
                AnalyzePrefabFile(prefabPath);
            }
        }
    }
    
    private void AnalyzePrefabFile(string prefabPath)
    {
        try
        {
            string prefabContent = File.ReadAllText(prefabPath);
            
            // Find script references in prefab
            MatchCollection scriptMatches = Regex.Matches(prefabContent, @"m_Script: \{fileID: \d+, guid: ([a-f0-9]+)");
            foreach (Match match in scriptMatches)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(scriptPath))
                {
                    usedFiles.Add(scriptPath);
                    AddFileDependency(prefabPath, scriptPath);
                }
            }
            
            // Find other asset references
            MatchCollection assetMatches = Regex.Matches(prefabContent, @"guid: ([a-f0-9]+)");
            foreach (Match match in assetMatches)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(assetPath))
                {
                    usedFiles.Add(assetPath);
                    AddFileDependency(prefabPath, assetPath);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not analyze prefab {prefabPath}: {e.Message}");
        }
    }
    
    private void AnalyzeScriptUsage()
    {
        Debug.Log("Analyzing script usage...");
        
        string[] scriptGuids = AssetDatabase.FindAssets("t:Script");
        foreach (string guid in scriptGuids)
        {
            string scriptPath = AssetDatabase.GUIDToAssetPath(guid);
            
            // All scripts that are attached to GameObjects or referenced by other scripts are considered used
            if (IsAssetReferenced(scriptPath) || usedFiles.Contains(scriptPath))
            {
                usedFiles.Add(scriptPath);
                AnalyzeScriptFile(scriptPath);
            }
        }
    }
    
    private void AnalyzeScriptFile(string scriptPath)
    {
        try
        {
            string scriptContent = File.ReadAllText(scriptPath);
            
            // Find using statements and class references
            MatchCollection usingMatches = Regex.Matches(scriptContent, @"using\s+([^;]+);");
            foreach (Match match in usingMatches)
            {
                string usingStatement = match.Groups[1].Value.Trim();
                // Note: This is basic analysis, more sophisticated dependency tracking could be added
            }
            
            // Find asset references in code (Resources.Load, etc.)
            MatchCollection resourceMatches = Regex.Matches(scriptContent, @"Resources\.Load[^(]*\(\s*[""']([^""']+)[""']");
            foreach (Match match in resourceMatches)
            {
                string resourcePath = match.Groups[1].Value;
                string fullPath = "Assets/Resources/" + resourcePath;
                if (File.Exists(fullPath))
                {
                    usedFiles.Add(fullPath);
                    AddFileDependency(scriptPath, fullPath);
                }
            }
            
            // Find direct file references in strings
            MatchCollection pathMatches = Regex.Matches(scriptContent, @"[""']Assets/[^""']+[""']");
            foreach (Match match in pathMatches)
            {
                string referencedPath = match.Value.Trim('"', '\'');
                if (File.Exists(referencedPath))
                {
                    usedFiles.Add(referencedPath);
                    AddFileDependency(scriptPath, referencedPath);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not analyze script {scriptPath}: {e.Message}");
        }
    }
    
    private void AnalyzeAssetUsage()
    {
        Debug.Log("Analyzing asset usage...");
        
        // Analyze materials
        string[] materialGuids = AssetDatabase.FindAssets("t:Material");
        foreach (string guid in materialGuids)
        {
            string materialPath = AssetDatabase.GUIDToAssetPath(guid);
            if (IsAssetReferenced(materialPath))
            {
                usedFiles.Add(materialPath);
                AnalyzeMaterialFile(materialPath);
            }
        }
        
        // Analyze textures
        string[] textureGuids = AssetDatabase.FindAssets("t:Texture");
        foreach (string guid in textureGuids)
        {
            string texturePath = AssetDatabase.GUIDToAssetPath(guid);
            if (IsAssetReferenced(texturePath))
            {
                usedFiles.Add(texturePath);
            }
        }
        
        // Analyze audio clips
        string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip");
        foreach (string guid in audioGuids)
        {
            string audioPath = AssetDatabase.GUIDToAssetPath(guid);
            if (IsAssetReferenced(audioPath))
            {
                usedFiles.Add(audioPath);
            }
        }
    }
    
    private void AnalyzeMaterialFile(string materialPath)
    {
        try
        {
            string materialContent = File.ReadAllText(materialPath);
            
            // Find texture references in materials
            MatchCollection textureMatches = Regex.Matches(materialContent, @"guid: ([a-f0-9]+).*?type: 2");
            foreach (Match match in textureMatches)
            {
                string texturePath = AssetDatabase.GUIDToAssetPath(match.Groups[1].Value);
                if (!string.IsNullOrEmpty(texturePath))
                {
                    usedFiles.Add(texturePath);
                    AddFileDependency(materialPath, texturePath);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Could not analyze material {materialPath}: {e.Message}");
        }
    }
    
    private void AnalyzeBuildSettings()
    {
        Debug.Log("Analyzing build settings...");
        
        // Mark all scenes in build settings as used
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        foreach (var scene in scenes)
        {
            if (!string.IsNullOrEmpty(scene.path))
            {
                usedFiles.Add(scene.path);
            }
        }
    }
    
    private void AnalyzeProjectSettings()
    {
        Debug.Log("Analyzing project settings...");
        
        // Add commonly referenced project files
        string[] projectFiles = {
            "Assets/StreamingAssets",
            "Assets/Resources",
            "Assets/Editor",
            "Assets/Plugins"
        };
        
        foreach (string dir in projectFiles)
        {
            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    string normalizedPath = file.Replace("\\", "/");
                    usedFiles.Add(normalizedPath);
                }
            }
        }
    }
    
    private bool IsAssetReferenced(string assetPath)
    {
        // Check if asset is referenced by any other file
        string assetGuid = AssetDatabase.AssetPathToGUID(assetPath);
        if (string.IsNullOrEmpty(assetGuid)) return false;
        
        foreach (string file in allProjectFiles)
        {
            if (file == assetPath) continue;
            
            try
            {
                if (File.Exists(file))
                {
                    string content = File.ReadAllText(file);
                    if (content.Contains(assetGuid))
                    {
                        return true;
                    }
                }
            }
            catch
            {
                // Skip files that can't be read
            }
        }
        
        return false;
    }
    
    private void AddFileDependency(string parentFile, string dependencyFile)
    {
        if (!fileDependencies.ContainsKey(parentFile))
        {
            fileDependencies[parentFile] = new List<string>();
        }
        
        if (!fileDependencies[parentFile].Contains(dependencyFile))
        {
            fileDependencies[parentFile].Add(dependencyFile);
        }
    }
    
    private void GenerateUnusedFilesList()
    {
        Debug.Log("Generating unused files list...");
        
        foreach (string file in allProjectFiles)
        {
            // Skip protected files
            if (protectedFiles.Contains(file)) continue;
            
            // Skip used files
            if (usedFiles.Contains(file)) continue;
            
            // This file appears to be unused
            unusedFiles.Add(file);
        }
        
        Debug.Log($"Found {unusedFiles.Count} potentially unused files");
    }
    
    private void CreateAnalysisReport()
    {
        string reportPath = "Assets/ProjectCleanupReport.txt";
        using (StreamWriter writer = new StreamWriter(reportPath))
        {
            writer.WriteLine("=== PROJECT CLEANUP ANALYSIS REPORT ===");
            writer.WriteLine($"Generated: {DateTime.Now}");
            writer.WriteLine();
            
            writer.WriteLine($"SUMMARY:");
            writer.WriteLine($"- Total project files: {allProjectFiles.Count}");
            writer.WriteLine($"- Protected files: {protectedFiles.Count}");
            writer.WriteLine($"- Used files: {usedFiles.Count}");
            writer.WriteLine($"- Potentially unused files: {unusedFiles.Count}");
            writer.WriteLine();
            
            if (includeDetailedReport)
            {
                writer.WriteLine("=== POTENTIALLY UNUSED FILES ===");
                foreach (string file in unusedFiles.OrderBy(f => f))
                {
                    writer.WriteLine($"- {file}");
                }
                writer.WriteLine();
                
                writer.WriteLine("=== PROTECTED FILES (DO NOT DELETE) ===");
                foreach (string file in protectedFiles.OrderBy(f => f).Take(50)) // Show first 50
                {
                    writer.WriteLine($"- {file}");
                }
                if (protectedFiles.Count > 50)
                {
                    writer.WriteLine($"... and {protectedFiles.Count - 50} more");
                }
                writer.WriteLine();
                
                if (fileDependencies.Count > 0)
                {
                    writer.WriteLine("=== FILE DEPENDENCIES (Sample) ===");
                    int depCount = 0;
                    foreach (var kvp in fileDependencies)
                    {
                        if (depCount >= 20) break; // Show first 20 dependencies
                        writer.WriteLine($"{kvp.Key}:");
                        foreach (string dep in kvp.Value.Take(5)) // Show first 5 deps per file
                        {
                            writer.WriteLine($"  -> {dep}");
                        }
                        if (kvp.Value.Count > 5)
                        {
                            writer.WriteLine($"  ... and {kvp.Value.Count - 5} more");
                        }
                        writer.WriteLine();
                        depCount++;
                    }
                }
            }
            
            writer.WriteLine("=== RECOMMENDATIONS ===");
            writer.WriteLine("1. Review the 'potentially unused files' list carefully");
            writer.WriteLine("2. Test your application thoroughly before deleting any files");
            writer.WriteLine("3. Create a backup before removing files");
            writer.WriteLine("4. Never delete files from the 'protected files' list");
            writer.WriteLine("5. Consider using Unity's 'Unused Assets' tool as a secondary check");
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"Analysis report saved to: {reportPath}");
        
        // Log summary to console
        Debug.Log("=== CLEANUP ANALYSIS SUMMARY ===");
        Debug.Log($"Total files: {allProjectFiles.Count}");
        Debug.Log($"Used files: {usedFiles.Count}");
        Debug.Log($"Potentially unused: {unusedFiles.Count}");
        Debug.Log($"Protected files: {protectedFiles.Count}");
        Debug.Log($"Report saved to: {reportPath}");
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ProjectCleanupAnalyzer))]
public class ProjectCleanupAnalyzerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        ProjectCleanupAnalyzer analyzer = (ProjectCleanupAnalyzer)target;
        
        if (GUILayout.Button("Analyze Project Files", GUILayout.Height(30)))
        {
            analyzer.AnalyzeProjectFiles();
        }
        
        EditorGUILayout.HelpBox(
            "This tool will analyze your entire project to identify unused files. " +
            "It will generate a detailed report with files that are safe to remove. " +
            "ALWAYS backup your project before deleting any files!", 
            MessageType.Warning);
        
        EditorGUILayout.HelpBox(
            "The analysis considers:\n" +
            "• Scene references\n" +
            "• Prefab dependencies\n" +
            "• Script usage\n" +
            "• Asset references\n" +
            "• Build settings\n" +
            "• Resources folder", 
            MessageType.Info);
    }
}
#endif
