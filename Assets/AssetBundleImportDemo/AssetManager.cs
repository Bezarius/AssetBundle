using System.IO;
using UnityEngine;
using UniRx;
using Object = UnityEngine.Object;

public class AssetManager : MonoBehaviour
{

    private IObservable<AssetBundleCreateRequest> RequestAssetBundle(string assetBundleName)
    {
        var assetBundlePath = Path.Combine(Application.streamingAssetsPath, assetBundleName);
        var asyncRequest = AssetBundle.LoadFromFileAsync(assetBundlePath);
        return asyncRequest.AsAsyncOperationObservable();
    }

    private IObservable<AssetBundleManifest> GetAssetBundleManifest(string assetBundleName)
    {
        var observable = Observable.Create<AssetBundleManifest>(observer =>
        {
            var assetBundleRequest = RequestAssetBundle(assetBundleName);
            assetBundleRequest.Subscribe(request =>
            {
                var assetBundle = request.assetBundle;
                var asyncRequest = assetBundle.LoadAssetAsync("AssetBundleManifest");
                asyncRequest.AsAsyncOperationObservable().Subscribe(manifestRequest =>
                {
                    var manifest = asyncRequest.asset as AssetBundleManifest;
                    observer.OnNext(manifest);
                    observer.OnCompleted();
                });
            });
            return Disposable.Empty;
        });
        return observable;
    }

    private IObservable<Object[]> GetAssetsFromBundle(string assetBundleName)
    {
        var observable = Observable.Create<Object[]>(observer =>
        {
            var assetBundleRequest = RequestAssetBundle(assetBundleName);
            assetBundleRequest.Subscribe(request =>
            {
                var asyncRequest = request.assetBundle.LoadAllAssetsAsync<GameObject>();
                asyncRequest.AsAsyncOperationObservable().Subscribe(bundleRequest =>
                {
                    observer.OnNext(bundleRequest.allAssets);
                    observer.OnCompleted();
                });
            });
            return Disposable.Empty;
        });

        return observable;
    }

    private void LoadAssets()
    {
        GetAssetBundleManifest("StreamingAssets").Subscribe(manifest =>
        {
            var allAssetNames = manifest.GetAllAssetBundles();
            foreach (var assetName in allAssetNames)
            {
                GetAssetsFromBundle(assetName).Subscribe(assets =>
                {
                    foreach (var asset in assets)
                    {
                        Instantiate(asset);
                    }
                });
            }
        });

    }

    void Start()
    {
        LoadAssets();
    }
}
