using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MissingScriptFinder : MonoBehaviour
{
    [ContextMenu("Find Missing Scripts in Scene")]
    public void FindMissingScripts()
    {
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        int missingCount = 0;
        
        foreach (GameObject go in allGameObjects)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.LogError($"Missing script found on GameObject: {go.name} at component index {i}", go);
                    missingCount++;
                }
            }
        }
        
        if (missingCount == 0)
        {
            Debug.Log("No missing scripts found in the scene!");
        }
        else
        {
            Debug.LogWarning($"Found {missingCount} missing script reference(s)");
        }
    }
    
    [ContextMenu("Remove Missing Scripts from Scene")]
    public void RemoveMissingScripts()
    {
        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        int removedCount = 0;
        
        foreach (GameObject go in allGameObjects)
        {
            // Use SerializedObject to safely remove missing scripts
            #if UNITY_EDITOR
            SerializedObject serializedObject = new SerializedObject(go);
            SerializedProperty prop = serializedObject.FindProperty("m_Component");
            
            for (int i = prop.arraySize - 1; i >= 0; i--)
            {
                SerializedProperty componentProp = prop.GetArrayElementAtIndex(i);
                SerializedProperty componentRef = componentProp.FindPropertyRelative("component");
                
                if (componentRef.objectReferenceValue == null)
                {
                    Debug.Log($"Removing missing script from: {go.name}", go);
                    prop.DeleteArrayElementAtIndex(i);
                    removedCount++;
                }
            }
            
            serializedObject.ApplyModifiedProperties();
            #endif
        }
        
        Debug.Log($"Removed {removedCount} missing script reference(s)");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MissingScriptFinder))]
public class MissingScriptFinderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        
        MissingScriptFinder finder = (MissingScriptFinder)target;
        
        if (GUILayout.Button("Find Missing Scripts"))
        {
            finder.FindMissingScripts();
        }
        
        if (GUILayout.Button("Remove Missing Scripts"))
        {
            finder.RemoveMissingScripts();
        }
    }
}
#endif
