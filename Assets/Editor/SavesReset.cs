using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ResetYGOnPlay
{
    private const string SavesReset = "SavesReset";

    static ResetYGOnPlay()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode && EditorPrefs.GetBool(SavesReset))
        {
            string path = Application.dataPath + "/PluginYourGames/Editor/SavesEditorYG2.json";

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
    }
}