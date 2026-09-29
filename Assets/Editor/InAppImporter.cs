using System;
using System.IO;
using Cards.Databases;
using UnityEditor;
using UnityEngine;

public class InAppImporter : EditorWindow
{
    private string csvPath = "Assets/Sources/InApps.csv";

    private InAppsDatabase targetSO;

    [MenuItem("Tools/CSV Importer")]
    public static void ShowWindow()
    {
        GetWindow<InAppImporter>("CSV Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Настройки импорта CSV", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        
        csvPath = EditorGUILayout.TextField("Путь к CSV:", csvPath);
        
        if (GUILayout.Button("Обзор", GUILayout.Width(60)))
        {
            string absolutePath = EditorUtility.OpenFilePanel("Выбрать CSV файл", "Assets", "csv");
            if (!string.IsNullOrEmpty(absolutePath))
            {
                if (absolutePath.StartsWith(Application.dataPath))
                    csvPath = "Assets" + absolutePath.Substring(Application.dataPath.Length);
                else
                    csvPath = absolutePath;                
            }
        }
        
        EditorGUILayout.EndHorizontal();

        targetSO = (InAppsDatabase)EditorGUILayout.ObjectField("Целевой SO:", targetSO, typeof(InAppsDatabase), false);

        GUILayout.Space(15);

        if (GUILayout.Button("Импортировать данные", GUILayout.Height(30)))
            ImportCsv();        
    }

    private void ImportCsv()
    {
        if (!File.Exists(csvPath))
        {
            Debug.LogError($"[CsvImporter] Файл не найден по пути: {csvPath}");
            return;
        }

        if (targetSO == null)
        {
            Debug.LogError("[CsvImporter] Укажите целевой ScriptableObject для записи данных!");
            return;
        }

        try
        {
            string[] lines = File.ReadAllLines(csvPath);
            
            if (lines.Length <= 1) 
                return;

            Undo.RecordObject(targetSO, "Import CSV Data");
            targetSO.Products.Clear();

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] values = line.Split(',');
                if (values.Length < 6) continue;

                targetSO.AddProductItem(values[0], int.Parse(values[1]), values[2], bool.Parse(values[6]));
            }

            EditorUtility.SetDirty(targetSO);
            AssetDatabase.SaveAssets();

            Debug.Log($"[CsvImporter] Данные успешно импортированы в {targetSO.name}!");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CsvImporter] Ошибка при импорте: {e.Message}");
        }
    }
}
