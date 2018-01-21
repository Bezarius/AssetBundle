using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles {

	[MenuItem ("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles ()
    {
		if(!Directory.Exists("Assets/StreamingAssets")){
			Directory.CreateDirectory("Assets/StreamingAssets");
		}
        BuildPipeline.BuildAssetBundles ("Assets/StreamingAssets", BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
    }
}
