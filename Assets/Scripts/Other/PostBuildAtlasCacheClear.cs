#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.Collections;

public class PostBuildAtlasCacheClear : MonoBehaviour {

    [PostProcessBuild]
    private static void DeleteAtlasCache(BuildTarget target, string pathToBuiltProject)
    {
        string projectPath = Application.dataPath; //Asset path
        string atlasCachePath = Path.GetFullPath(Path.Combine(projectPath, Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Library" + Path.DirectorySeparatorChar + "AtlasCache"));
        Directory.Delete(atlasCachePath,true);
        Debug.Log("Deleted atlas cache folder.");
    }
}
#endif
