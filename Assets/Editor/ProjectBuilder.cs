using System;
using System.IO;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Aurora.EditorTools
{
    public static class ProjectBuilder
    {
        public const string ScenePath = "Assets/Scenes/Bootstrap.unity";
        [MenuItem("Ecos de Aurora/Preparar projeto")]
        public static void Prepare()
        {
            Directory.CreateDirectory("Assets/Scenes");
            foreach (string file in Directory.GetFiles("Assets/Resources/Art", "*.png"))
            {
                var importer = AssetImporter.GetAtPath(file.Replace('\\', '/')) as TextureImporter;
                if (importer == null) continue;
                importer.textureType = TextureImporterType.Default;
                importer.filterMode = FilterMode.Point; importer.mipmapEnabled = false;
                importer.textureCompression = TextureImporterCompression.Uncompressed;
                importer.alphaIsTransparency = true; importer.npotScale = TextureImporterNPOTScale.None;
                importer.SaveAndReimport();
            }
            PlayerSettings.companyName = "Pixel Academy"; PlayerSettings.productName = "Ecos de Aurora";
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
            PlayerSettings.runInBackground = false;
            // Input legado é intencional neste projeto didático; não exige pacote Input System.
            var settings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var input = settings.FindProperty("activeInputHandler"); if (input != null) { input.intValue = 0; settings.ApplyModifiedPropertiesWithoutUndo(); }
            if (!File.Exists(ScenePath))
            {
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                var cameraObject = new GameObject("Main Camera"); cameraObject.tag = "MainCamera";
                var camera = cameraObject.AddComponent<Camera>(); camera.orthographic = true; camera.orthographicSize = 5.6f; camera.backgroundColor = new Color(.035f, .065f, .09f); camera.clearFlags = CameraClearFlags.SolidColor;
                cameraObject.AddComponent<AudioListener>(); cameraObject.transform.position = new Vector3(0,0,-10);
                EditorSceneManager.SaveScene(scene, ScenePath);
            }
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets(); AssetDatabase.Refresh();
            Debug.Log("Ecos de Aurora preparado. Abra Bootstrap e pressione Play.");
        }
        [MenuItem("Ecos de Aurora/Build Windows")]
        public static void BuildWindows() { Build(BuildTarget.StandaloneWindows64, "Builds/Windows/EcosDeAurora.exe"); }
        [MenuItem("Ecos de Aurora/Build Linux")]
        public static void BuildLinux() { Build(BuildTarget.StandaloneLinux64, "Builds/Linux/EcosDeAurora.x86_64"); }
        static void Build(BuildTarget target, string output)
        {
            Prepare();
            if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, target)) throw new InvalidOperationException("Instale o módulo de build " + target + " pelo Unity Hub.");
            Directory.CreateDirectory(Path.GetDirectoryName(output));
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { ScenePath }, locationPathName = output, target = target, options = BuildOptions.None });
            if (report.summary.result != BuildResult.Succeeded) throw new InvalidOperationException("Build falhou: " + report.summary.totalErrors + " erros.");
            Debug.Log("Build concluído: " + output);
        }
    }
}
