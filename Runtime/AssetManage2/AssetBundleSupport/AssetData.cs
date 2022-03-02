using System;

namespace Litchi.AssetManage2
{
    [Serializable]
    public class AssetData
    {
        public string assetName { get; set; }
        public string assetBundleName  { get; set; }
        public int assetBundleIndex { get; set; }
        public short assetType { get; set; }
        public short assetObjectTypeCode  { get; set; } = 0;

        public string UUID
        {
            get 
            {
                return string.IsNullOrEmpty(assetBundleName) ? assetName : assetBundleName + assetName;
            }
        }

        public AssetData(string assetName, short assetType, int assetBundleIndex, string assetBundleName, short assetObjectTypeCode = 0)
        {
            this.assetName = assetName;
            this.assetType = assetType;
            this.assetBundleIndex = assetBundleIndex;
            this.assetBundleName = assetBundleName;
            this.assetObjectTypeCode = assetObjectTypeCode;
        }
    }
}