using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;

namespace Litchi.AssetManage2
{
    public interface IAssetLoader
    {
        // void Add2Load(string assetName, Action<bool, IAsset> listener, bool lastOrder = true);
        // void Add2Load(string assetName, string assetBundleName, Action<bool, IAsset> listener, bool lastOrder = true);
        void ReleaseAllAssets();
        // void UnloadAllInstantiateAssets();
    }

    public class AssetLoader : IAssetLoader, IPoolable
    {

        public T LoadSync<T>(string assetName) where T : Object
        {
            if(typeof(T) == typeof(Sprite))
            {
                Object obj = null;
                return obj as T;
            }
            else
            {
                var key = AssetSearchKey.Allocate(assetName, null, typeof(T));
                var retAsset = LoadAssetSync(key);
                key.Recycle();
                return retAsset.asset as T;
            }
        }

        public IAsset LoadAssetSync(AssetSearchKey key)
        {
            Add2Load(key);
            LoadSync();

            var asset = AssetManager.instance.GetAsset(key);
            if(asset == null)
            {

            }
            else
            {

            }
            return asset;
        }

        private void LoadSync()
        {
            while(m_WaitLoadList.Count > 0)
            {
                var first = m_WaitLoadList.First.Value;
                -- m_LoadingCount;
                m_WaitLoadList.RemoveFirst();
                if(first == null) return;
                if(first.LoadSync())
                {

                }
            }
        }

        List<string> tempDependencies = new List<string>();
        private readonly List<IAsset> m_AssetList = new List<IAsset>();
        private readonly LinkedList<IAsset> m_WaitLoadList = new LinkedList<IAsset>();

        private int m_LoadingCount;

        private void Add2Load(AssetSearchKey key, Action<bool, IAsset> listener = null, bool lastOrder = true)
        {
            IAsset asset = FindAssetInArray(m_AssetList, key);
            if(asset != null)
            {
                if(listener != null)
                {
                    // marktodo
                    // asset.
                }
                return;
            }

            asset = AssetManager.instance.GetOrCreateAsset(key);

            if(asset == null)
            {
                return;
            }

            if(listener != null)
            {

            }

            var dependencies = asset.GetDependencies();
            if(dependencies != null)
            {
                foreach (var item in dependencies)
                {
                    if(!tempDependencies.Contains(item))
                    {
                        var searchKey = AssetSearchKey.Allocate(item, null, typeof(AssetBundle));
                        tempDependencies.Add(item);
                        Add2Load(searchKey);
                        searchKey.Recycle();
                    }
                }
            }

            AddAsset2Array(asset, lastOrder);
        }

        private void AddAsset2Array(IAsset asset, bool lastOrder)
        {
            var key = AssetSearchKey.Allocate(asset.assetName, asset.assetBundleName, asset.assetType);
            var oldAsset = FindAssetInArray(m_AssetList, key);
            key.Recycle();

            if(oldAsset != null)
            {
                return;
            }

            asset.Retain();
            m_AssetList.Add(asset);

            if(asset.state != AssetState.Ready)
            {
                ++ m_LoadingCount;
                if(lastOrder)
                {
                    m_WaitLoadList.AddLast(asset);
                }
                else
                {
                    m_WaitLoadList.AddFirst(asset);
                }
            }
        }

        private static IAsset FindAssetInArray(List<IAsset> list, AssetSearchKey key)
        {
            if(list == null) return null;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if(key.Match(list[i]))
                {
                    return list[i];
                }
            }
            return null;
        }

        public void ReleaseAllAssets()
        {

        }

        #region IPoolable

        bool IPoolable.isRecycled { get; set; }
        void IPoolable.OnRecycled()
        {
            ReleaseAllAssets();
        }

        #endregion
    }
}