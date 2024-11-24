using System.IO;
using UnityEditor;
using UnityEngine;

public class CopyAssets : AssetPostprocessor
{
    private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        // Specify the source and destination paths
        string sourcePath = "Packages/com.kraken.developer-console/Resources/";
        string destinationPath = "Assets/Resources/";

        // Ensure the destination folder exists
        if (!Directory.Exists(destinationPath))
        {
            Directory.CreateDirectory(destinationPath);
        }

        // Copy the assets
        foreach (string assetPath in importedAssets)
        {
            if (assetPath.StartsWith(sourcePath))
            {
                string relativePath = assetPath.Substring(sourcePath.Length);
                string destinationAssetPath = Path.Combine(destinationPath, relativePath);
                File.Copy(assetPath, destinationAssetPath, true);
            }
        }
    }
}