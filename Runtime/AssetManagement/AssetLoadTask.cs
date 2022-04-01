using System;
using System.Collections;
using Object = UnityEngine.Object;

namespace Litchi.AssetManagement
{
    public class AssetLoadTask : ExecuteTask
    {
        public Action<Object> onCompleted = delegate {};

        public string path { get; private set; }
        public Type type { get; private set; }
        private IAssetLoader m_AssetLoader;
        private Object m_Asset;

        public AssetLoadTask(string path, Type type, IAssetLoader assetLoader)
        {
            this.path = path;
            this.type = type;
            m_AssetLoader = assetLoader;
        }

        public override IEnumerator OnExecute() 
        {
            yield return m_AssetLoader.LoadAsync(path, type, asset => {
                this.m_Asset = asset;
            });
            onCompleted(this.m_Asset);
        }
    }
}