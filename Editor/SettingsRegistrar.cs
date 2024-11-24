using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Kraken;

static class SettingsRegistrar
{
    [SettingsProvider]
    public static SettingsProvider CreateDeveloperConsoleSettingsProvider()
    {
        var provider = new SettingsProvider("Project/Kraken Developer Console", SettingsScope.Project)
        {
            label = "Kraken Developer Console",
            guiHandler = (searchContext) =>
            {
                var settings = GetSerializedSettings();
                EditorGUILayout.PropertyField(settings.FindProperty("_consoleUIMode"), new GUIContent("UI Mode"));
                EditorGUILayout.PropertyField(settings.FindProperty("_generateLogFile"), new GUIContent("Generate Log File"));
                EditorGUILayout.PropertyField(settings.FindProperty("_infoColor"), new GUIContent("Info Color"));
                EditorGUILayout.PropertyField(settings.FindProperty("_warningColor"), new GUIContent("Warning Color"));
                EditorGUILayout.PropertyField(settings.FindProperty("_errorColor"), new GUIContent("Error Color"));
                EditorGUILayout.PropertyField(settings.FindProperty("_assertColor"), new GUIContent("Assert Color"));
                EditorGUILayout.PropertyField(settings.FindProperty("_exceptionColor"), new GUIContent("Exception Color"));
                settings.ApplyModifiedPropertiesWithoutUndo();
                if (settings.FindProperty("_generateLogFile").boolValue)
                {
                    PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, "KRAKEN_ENABLE_LOG_FILE_GEN");
                }
                else
                {
                    PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, out var defines);
                    var newDefines = defines.ToList().Where(x => !string.Equals(x, "KRAKEN_ENABLE_LOG_FILE_GEN")).ToArray();
                    PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Standalone, newDefines);
                }
            },
            
            keywords = new HashSet<string>(new[]{"Kraken","Developer","Console"})
        };
        
        return provider;
    }
    
    internal static DeveloperConsoleSettings GetSettingsAsset()
    {
        var settings = AssetDatabase.LoadAssetAtPath<DeveloperConsoleSettings>(DeveloperConsoleSettings.settingsAssetPath);
        if (settings == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            {
                AssetDatabase.CreateFolder("Assets", "Settings");
            }
            settings = ScriptableObject.CreateInstance<DeveloperConsoleSettings>();
            AssetDatabase.CreateAsset(settings, DeveloperConsoleSettings.settingsAssetPath);
            AssetDatabase.SaveAssets();
        }
        return settings;
    }

    internal static SerializedObject GetSerializedSettings()
    {
        return new SerializedObject(GetSettingsAsset());
    }
}
