using System;

namespace Litchi.AssetManage2
{
    public class AssetSearchKey : IPoolable
    {
        public string assetName { get; set; }
        public string assetBundleName { get; set; }
        public Type assetType { get; set; }

        public static AssetSearchKey Allocate(string assetName, string assetBundleName = null, Type assetType = null)
        {
            var key = ObjectPool<AssetSearchKey>.instance.Allocate();
            key.assetName = assetName;
            key.assetBundleName = assetBundleName;
            key.assetType = assetType;
            return key;
        }

        public void Recycle()
        {
            ObjectPool<AssetSearchKey>.instance.Recycle(this);
        }

        public bool Match(IAsset asset)
        {
            if(asset.assetName == assetName)
            {
                var isMatch = true;
                if(assetType != null)
                {
                    isMatch = asset.assetType == assetType;
                }

                if(assetBundleName != null)
                {
                    isMatch = isMatch && asset.assetBundleName == assetBundleName;
                }
                return isMatch;
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("AssetName:{0} AssetBundleName:{1} AssetType:{2}", assetName, assetBundleName,
                assetType);
        }

        void IPoolable.OnRecycled()
        {
            assetName = null;
            assetBundleName = null;
            assetType = null;
        }

        bool IPoolable.isRecycled { get; set; }
    }
}