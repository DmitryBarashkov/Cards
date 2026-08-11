using UnityEditor;

public static class SavesResetFlag
{
    private const string MenuPath = "EditorFlags/SavesResetFlag/Enabled";
    private const string SavesReset = "SavesReset";

    [MenuItem(MenuPath)]
    private static void Toggle()
    {
        bool result = EditorPrefs.GetBool(SavesReset);

        EditorPrefs.SetBool(SavesReset, !result);
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleValidate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(SavesReset));

        return true;
    }
}