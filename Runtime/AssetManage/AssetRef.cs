using UnityEngine;

namespace Litchi.AssetManage
{
    public interface IAssetRef : IRefCounter
    {
        string assetPath {get;}
        Object assetObject {get;}
    }

    public class AssetRef : IAssetRef
    {
        public AssetRef(string assetPath, Object assetObject)
        {
            this.assetPath = assetPath;
            this.assetObject = assetObject;
        }

        public string assetPath {get; protected set;}

        public Object assetObject {get; protected set;}

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