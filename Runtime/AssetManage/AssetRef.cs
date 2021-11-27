using UnityEngine;

namespace Litchi.AssetManage
{
    public interface IAssetRef : IRefCounter
    {
        string assetPath {get;}
        Object asset {get;}
    }

    public class AssetRef : IAssetRef
    {
        public AssetRef(string assetPath, Object asset)
        {
            this.assetPath = assetPath;
            this.asset = asset;
        }

        public string assetPath {get; protected set;}

        public Object asset {get; protected set;}

        public int refCount {get; protected set;}
        public void Retain()
        {
            refCount ++;
        }

        public void Release()
        {
            refCount --;
        }
    }
}