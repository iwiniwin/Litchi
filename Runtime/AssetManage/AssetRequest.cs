using UnityEngine;

namespace Litchi.AssetManage
{
    public class AssetRequest : AsyncOperation
    {
        public AssetRequest()
        {
            
        }

        public Object asset { get; }
        public Object[] allAssets { get; }
    }
}