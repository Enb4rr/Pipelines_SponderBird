using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class CIBuilder
{
    [MenuItem("CI/BuildWebGL")]
    public static void PerformWebGLBuild()
    {
        try
        {
            string[] scenes = EditorBuildSettings.scenes
                .Where(s => s.enabled)
                .Select(s => s.path)
                .ToArray();

            if (scenes.Length == 0)
            {
                Debug.LogError("No WebGL scenes found.");
                return;
            }

            string buildPath = "Builds/WebGL";
            if (!Directory.Exists(buildPath))
            {
                Directory.CreateDirectory(buildPath);
                Debug.Log("Created build path: " + buildPath);
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = scenes;
            buildPlayerOptions.locationPathName = buildPath;
            buildPlayerOptions.target = BuildTarget.WebGL;
            buildPlayerOptions.options = BuildOptions.None;

            UnityEditor.Build.Reporting.BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            UnityEditor.Build.Reporting.BuildSummary summary = report.summary;

            if (summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log("Build succeeded: " + summary.totalSize + " byte(s)");
            }
            else if (summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            {
                Debug.Log("Build failed: " + summary.totalErrors + " error(s).");
            }
        }
        catch (Exception e)
        {
            Debug.Log($"Critical error {e.Message}");
        }
    }
}
