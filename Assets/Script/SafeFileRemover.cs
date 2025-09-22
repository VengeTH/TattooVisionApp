using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SafeFileRemover : MonoBehaviour
{
    [Header("Safe File Removal Tool")]
    [SerializeField] public bool createBackupBeforeRemoval = true;
    [SerializeField] public string backupDirectory = "ProjectBackup";
    [SerializeField] public bool dryRun = true; // Simulate removal without actually deleting
    
    private List<string> filesToRemove = new List<string>();
    private List<string> backedUpFiles = new List<string>();
    
    [ContextMenu("Load Unused Files from Report")]
    public void LoadUnusedFilesFromReport()
    {
        #if UNITY_EDITOR
        string reportPath = "Assets/ProjectCleanupReport.txt";
        
        if (!File.Exists(reportPath))
        {
            Debug.LogError("Project cleanup report not found! Run the ProjectCleanupAnalyzer first.");
            return;
        }
        
        filesToRemove.Clear();
        
        try
        {
            string[] lines = File.ReadAllLines(reportPath);
            bool inUnusedSection = false;
            
            foreach (string line in lines)
            {
                if (line.Contains("=== POTENTIALLY UNUSED FILES ==="))
                {
                    inUnusedSection = true;
                    continue;
                }
                
                if (line.Contains("===") && inUnusedSection)
                {
                    break; // End of unused files section
                }
                
                if (inUnusedSection && line.StartsWith("- "))
                {
                    string filePath = line.Substring(2).Trim();
                    if (File.Exists(filePath))
                    {
                        filesToRemove.Add(filePath);
                    }
                }
            }
            
            Debug.Log($"Loaded {filesToRemove.Count} potentially unused files from report");
            
            // Show preview of files to be removed
            if (filesToRemove.Count > 0)
            {
                Debug.Log("=== FILES MARKED FOR REMOVAL ===");
                int previewCount = Math.Min(20, filesToRemove.Count);
                for (int i = 0; i < previewCount; i++)
                {
                    Debug.Log($"- {filesToRemove[i]}");
                }
                if (filesToRemove.Count > previewCount)
                {
                    Debug.Log($"... and {filesToRemove.Count - previewCount} more files");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error reading cleanup report: {e.Message}");
        }
        #endif
    }
    
    [ContextMenu("Remove Selected Files (Safe)")]
    public void RemoveSelectedFiles()
    {
        #if UNITY_EDITOR
        if (filesToRemove.Count == 0)
        {
            Debug.LogWarning("No files loaded for removal. Use 'Load Unused Files from Report' first.");
            return;
        }
        
        Debug.Log($"Starting {(dryRun ? "DRY RUN " : "ACTUAL ")}removal of {filesToRemove.Count} files...");
        
        if (createBackupBeforeRemoval && !dryRun)
        {
            CreateBackup();
        }
        
        List<string> successfullyRemoved = new List<string>();
        List<string> failedToRemove = new List<string>();
        
        foreach (string filePath in filesToRemove)
        {
            try
            {
                if (ShouldKeepFile(filePath))
                {
                    Debug.Log($"KEEPING (safety check failed): {filePath}");
                    continue;
                }
                
                if (dryRun)
                {
                    Debug.Log($"DRY RUN - Would remove: {filePath}");
                    successfullyRemoved.Add(filePath);
                }
                else
                {
                    // Actually remove the file
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                        
                        // Also remove .meta file if it exists
                        string metaFile = filePath + ".meta";
                        if (File.Exists(metaFile))
                        {
                            File.Delete(metaFile);
                        }
                        
                        successfullyRemoved.Add(filePath);
                        Debug.Log($"REMOVED: {filePath}");
                    }
                }
            }
            catch (Exception e)
            {
                failedToRemove.Add(filePath);
                Debug.LogError($"Failed to remove {filePath}: {e.Message}");
            }
        }
        
        // Summary
        Debug.Log("=== REMOVAL SUMMARY ===");
        Debug.Log($"Files processed: {filesToRemove.Count}");
        Debug.Log($"Successfully {(dryRun ? "would be " : "")}removed: {successfullyRemoved.Count}");
        Debug.Log($"Failed to remove: {failedToRemove.Count}");
        Debug.Log($"Mode: {(dryRun ? "DRY RUN (no files actually deleted)" : "ACTUAL REMOVAL")}");
        
        if (!dryRun && successfullyRemoved.Count > 0)
        {
            AssetDatabase.Refresh();
            Debug.Log("Asset database refreshed");
            
            if (createBackupBeforeRemoval)
            {
                Debug.Log($"Backup created at: {Path.Combine(Application.dataPath, "..", backupDirectory)}");
            }
        }
        
        // Save removal log
        SaveRemovalLog(successfullyRemoved, failedToRemove);
        #endif
    }
    
    #if UNITY_EDITOR
    private bool ShouldKeepFile(string filePath)
    {
        // Additional safety checks before deletion
        
        // Never delete currently open scenes
        for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
        {
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);
            if (scene.path == filePath)
            {
                Debug.LogWarning($"File is currently open scene, keeping: {filePath}");
                return true;
            }
        }
        
        // Check if file is in build settings
        EditorBuildSettingsScene[] buildScenes = EditorBuildSettings.scenes;
        foreach (var scene in buildScenes)
        {
            if (scene.path == filePath)
            {
                Debug.LogWarning($"File is in build settings, keeping: {filePath}");
                return true;
            }
        }
        
        // Keep essential directories
        if (filePath.StartsWith("Assets/Resources/") ||
            filePath.StartsWith("Assets/StreamingAssets/") ||
            filePath.StartsWith("Assets/Editor/") ||
            filePath.StartsWith("Assets/Plugins/"))
        {
            Debug.LogWarning($"File is in essential directory, keeping: {filePath}");
            return true;
        }
        
        // Keep files modified recently (last 24 hours) as extra safety
        if (File.Exists(filePath))
        {
            DateTime lastWrite = File.GetLastWriteTime(filePath);
            if ((DateTime.Now - lastWrite).TotalHours < 24)
            {
                Debug.LogWarning($"File was modified recently, keeping: {filePath}");
                return true;
            }
        }
        
        // Keep any file that contains "important", "critical", "main", etc. in the name
        string fileName = Path.GetFileName(filePath).ToLower();
        string[] importantKeywords = { "main", "manager", "controller", "important", "critical", "essential", "core" };
        foreach (string keyword in importantKeywords)
        {
            if (fileName.Contains(keyword))
            {
                Debug.LogWarning($"File name suggests importance, keeping: {filePath}");
                return true;
            }
        }
        
        return false;
    }
    
    private void CreateBackup()
    {
        Debug.Log("Creating backup of files to be removed...");
        
        string backupPath = Path.Combine(Application.dataPath, "..", backupDirectory);
        if (!Directory.Exists(backupPath))
        {
            Directory.CreateDirectory(backupPath);
        }
        
        string timestampedBackupPath = Path.Combine(backupPath, $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}");
        Directory.CreateDirectory(timestampedBackupPath);
        
        backedUpFiles.Clear();
        
        foreach (string filePath in filesToRemove)
        {
            if (!File.Exists(filePath)) continue;
            
            try
            {
                // Create subdirectory structure in backup
                string relativePath = filePath.Replace("Assets/", "");
                string backupFilePath = Path.Combine(timestampedBackupPath, relativePath);
                string backupDir = Path.GetDirectoryName(backupFilePath);
                
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }
                
                // Copy file to backup
                File.Copy(filePath, backupFilePath);
                
                // Also backup .meta file if it exists
                string metaFile = filePath + ".meta";
                if (File.Exists(metaFile))
                {
                    File.Copy(metaFile, backupFilePath + ".meta");
                }
                
                backedUpFiles.Add(filePath);
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to backup {filePath}: {e.Message}");
            }
        }
        
        Debug.Log($"Backed up {backedUpFiles.Count} files to: {timestampedBackupPath}");
    }
    
    private void SaveRemovalLog(List<string> removed, List<string> failed)
    {
        string logPath = "Assets/FileRemovalLog.txt";
        
        using (StreamWriter writer = new StreamWriter(logPath))
        {
            writer.WriteLine("=== FILE REMOVAL LOG ===");
            writer.WriteLine($"Date: {DateTime.Now}");
            writer.WriteLine($"Mode: {(dryRun ? "DRY RUN" : "ACTUAL REMOVAL")}");
            writer.WriteLine($"Backup created: {(createBackupBeforeRemoval && !dryRun)}");
            writer.WriteLine();
            
            writer.WriteLine($"SUMMARY:");
            writer.WriteLine($"- Total files processed: {filesToRemove.Count}");
            writer.WriteLine($"- Successfully removed: {removed.Count}");
            writer.WriteLine($"- Failed to remove: {failed.Count}");
            writer.WriteLine();
            
            if (removed.Count > 0)
            {
                writer.WriteLine("=== SUCCESSFULLY REMOVED FILES ===");
                foreach (string file in removed.OrderBy(f => f))
                {
                    writer.WriteLine($"- {file}");
                }
                writer.WriteLine();
            }
            
            if (failed.Count > 0)
            {
                writer.WriteLine("=== FAILED TO REMOVE ===");
                foreach (string file in failed.OrderBy(f => f))
                {
                    writer.WriteLine($"- {file}");
                }
                writer.WriteLine();
            }
            
            writer.WriteLine("=== NEXT STEPS ===");
            if (dryRun)
            {
                writer.WriteLine("1. This was a dry run - no files were actually deleted");
                writer.WriteLine("2. Review the list above carefully");
                writer.WriteLine("3. Disable 'Dry Run' and run again to actually remove files");
                writer.WriteLine("4. Make sure to test your application after removal");
            }
            else
            {
                writer.WriteLine("1. Files have been permanently removed");
                writer.WriteLine("2. Test your application thoroughly");
                writer.WriteLine("3. If issues occur, restore from backup");
                writer.WriteLine("4. Run the ProjectCleanupAnalyzer again to verify cleanup");
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"Removal log saved to: {logPath}");
    }
    
    [ContextMenu("Restore From Backup")]
    public void RestoreFromBackup()
    {
        string backupPath = Path.Combine(Application.dataPath, "..", backupDirectory);
        
        if (!Directory.Exists(backupPath))
        {
            Debug.LogError("No backup directory found!");
            return;
        }
        
        string[] backupDirs = Directory.GetDirectories(backupPath);
        if (backupDirs.Length == 0)
        {
            Debug.LogError("No backup folders found!");
            return;
        }
        
        // Use the most recent backup
        string latestBackup = backupDirs.OrderByDescending(d => Directory.GetCreationTime(d)).First();
        
        Debug.Log($"Restoring from backup: {latestBackup}");
        
        string[] backedUpFiles = Directory.GetFiles(latestBackup, "*", SearchOption.AllDirectories);
        int restoredCount = 0;
        
        foreach (string backupFile in backedUpFiles)
        {
            try
            {
                string relativePath = Path.GetRelativePath(latestBackup, backupFile);
                string targetPath = Path.Combine("Assets", relativePath);
                string targetDir = Path.GetDirectoryName(targetPath);
                
                if (!Directory.Exists(targetDir))
                {
                    Directory.CreateDirectory(targetDir);
                }
                
                File.Copy(backupFile, targetPath, true);
                restoredCount++;
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Failed to restore {backupFile}: {e.Message}");
            }
        }
        
        AssetDatabase.Refresh();
        Debug.Log($"Restored {restoredCount} files from backup");
    }
    #endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(SafeFileRemover))]
public class SafeFileRemoverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        SafeFileRemover remover = (SafeFileRemover)target;
        
        EditorGUILayout.HelpBox(
            "STEP 1: Run ProjectCleanupAnalyzer first to generate the cleanup report.", 
            MessageType.Info);
        
        if (GUILayout.Button("Load Unused Files from Report"))
        {
            remover.LoadUnusedFilesFromReport();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.HelpBox(
            "STEP 2: Review the loaded files, then remove them safely.", 
            MessageType.Warning);
        
        if (GUILayout.Button("Remove Files (Safe)", GUILayout.Height(30)))
        {
            if (EditorUtility.DisplayDialog("Confirm File Removal", 
                "Are you sure you want to proceed with file removal? " +
                (remover.dryRun ? "(This is a DRY RUN - no files will actually be deleted)" : 
                "This will PERMANENTLY delete the selected files!"), 
                "Yes", "Cancel"))
            {
                remover.RemoveSelectedFiles();
            }
        }
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Restore From Backup"))
        {
            if (EditorUtility.DisplayDialog("Confirm Restore", 
                "This will restore files from the most recent backup. Continue?", 
                "Yes", "Cancel"))
            {
                remover.RestoreFromBackup();
            }
        }
        
        EditorGUILayout.HelpBox(
            "SAFETY FEATURES:\n" +
            "• Dry run mode by default\n" +
            "• Automatic backup creation\n" +
            "• Additional safety checks\n" +
            "• Detailed removal logging\n" +
            "• One-click restore functionality", 
            MessageType.Info);
    }
}
#endif
