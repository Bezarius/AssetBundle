using System.IO;
using UnityEngine;
using UniRx;

public class AssetManager : MonoBehaviour {
    private AssetBundle GetAssetBundle(string assetBundleName){
		var assetBundlePath = Path.Combine(Application.streamingAssetsPath, assetBundleName);
		var asyncRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
		asyncRequest.AsAsyncOperationObservable();
		return asyncRequest.assetBundle;
	}

	private AssetBundleManifest GetAssetBundleManifest(string assetBundleName){
		var assetBundle = GetAssetBundle(assetBundleName);
    	var asyncRequest = assetBundle.LoadAssetAsync("AssetBundleManifest");
		asyncRequest.AsAsyncOperationObservable();
    	return asyncRequest.asset as AssetBundleManifest;
	}

	private UnityEngine.Object[] GetAssetsFromBundle(string assetBundleName)
    {
		var assetBundle = GetAssetBundle(assetBundleName);

		var asyncRequest = assetBundle.LoadAllAssetsAsync<GameObject>();
		asyncRequest.AsAsyncOperationObservable();

		return asyncRequest.allAssets;
    }

	void Start () {
		var time = Time.time;
		var manifest = GetAssetBundleManifest("StreamingAssets");
		var allAssetNames = manifest.GetAllAssetBundles();
		for(int i = 0; i < allAssetNames.Length; i++){
			var assets = GetAssetsFromBundle(allAssetNames[i]);
			for(int j = 0; j < assets.Length; j++){
				Instantiate(assets[j]);
			}
		}
		Debug.Log(Time.time - time);
	}
}
