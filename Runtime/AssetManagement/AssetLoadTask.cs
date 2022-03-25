using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetLoadTask : ExecuteTask
    {
        public Action<Object> onCompleted = delegate {};

        public ulong hash { get; private set; }
        public Type type { get; private set; }
        private IAssetLoader m_AssetLoader;
        private Object m_Asset;

        public AssetLoadTask(ulong hash, Type type, IAssetLoader assetLoader)
        {
            this.hash = hash;
            this.type = type;
            m_AssetLoader = assetLoader;
        }

        public override IEnumerator OnExecute() 
        {
            yield return m_AssetLoader.LoadAsync(hash, type, asset => {
                this.m_Asset = asset;
            });
            onCompleted(this.m_Asset);
        }
    }
}