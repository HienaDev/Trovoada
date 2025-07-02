using UnityEngine;
using UnityEditor;

public class ObjectNameReplacer : EditorWindow
{
    private string searchText = "batman";
    private string replaceText = "ball";
    private bool includeInactive = true;

    [MenuItem("Tools/Object Name Replacer")]
    public static void ShowWindow()
    {
        GetWindow<ObjectNameReplacer>("Name Replacer");
    }

    void OnGUI()
    {
        GUILayout.Label("Replace Object Names", EditorStyles.boldLabel);

        searchText = EditorGUILayout.TextField("Search for:", searchText);
        replaceText = EditorGUILayout.TextField("Replace with:", replaceText);
        includeInactive = EditorGUILayout.Toggle("Include inactive objects", includeInactive);

        GUILayout.Space(10);

        if (GUILayout.Button("Replace Names"))
        {
            ReplaceObjectNames();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Preview Changes"))
        {
            PreviewChanges();
        }
    }

    void ReplaceObjectNames()
    {
        if (string.IsNullOrEmpty(searchText))
        {
            EditorUtility.DisplayDialog("Error", "Search text cannot be empty!", "OK");
            return;
        }

        GameObject[] allObjects = includeInactive ?
            Resources.FindObjectsOfTypeAll<GameObject>() :
            FindObjectsOfType<GameObject>();

        int replacedCount = 0;

        // Record undo for all changes
        Undo.RegisterCompleteObjectUndo(allObjects, "Replace Object Names");

        foreach (GameObject obj in allObjects)
        {
            // Skip prefab assets and objects not in the scene
            if (EditorUtility.IsPersistent(obj)) continue;

            if (obj.name.Contains(searchText))
            {
                string newName = obj.name.Replace(searchText, replaceText);
                obj.name = newName;
                replacedCount++;

                // Mark the scene as dirty so Unity knows to save changes
                EditorUtility.SetDirty(obj);
            }
        }

        Debug.Log($"Replaced names in {replacedCount} objects. '{searchText}' -> '{replaceText}'");
        EditorUtility.DisplayDialog("Complete", $"Replaced names in {replacedCount} objects.", "OK");
    }

    void PreviewChanges()
    {
        if (string.IsNullOrEmpty(searchText))
        {
            EditorUtility.DisplayDialog("Error", "Search text cannot be empty!", "OK");
            return;
        }

        GameObject[] allObjects = includeInactive ?
            Resources.FindObjectsOfTypeAll<GameObject>() :
            FindObjectsOfType<GameObject>();

        Debug.Log("=== PREVIEW: Objects that would be renamed ===");
        int previewCount = 0;

        foreach (GameObject obj in allObjects)
        {
            if (EditorUtility.IsPersistent(obj)) continue;

            if (obj.name.Contains(searchText))
            {
                string newName = obj.name.Replace(searchText, replaceText);
                Debug.Log($"'{obj.name}' -> '{newName}'");
                previewCount++;
            }
        }

        if (previewCount == 0)
        {
            Debug.Log("No objects found containing the search text.");
        }

        Debug.Log($"=== {previewCount} objects would be renamed ===");
    }
}