using UnityEngine;
using System;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using UnityEngine.SceneManagement;

namespace Litchi.AssetManage2
{
    public interface IAssetLoader
    {
        void Add2Load(string assetName, Action<bool, IAsset> listener, bool lastOrder = true);
        void Add2Load(string assetName, string assetBundleName, Action<bool, IAsset> listener, bool lastOrder = true);
        void ReleaseAllAssets();
        // void UnloadAllInstantiateAssets();
    }

    public class AssetLoader : IAssetLoader, IPoolable
    {

        public T LoadSync<T>(string assetName) where T : Object
        {
            if (typeof(T) == typeof(Sprite))
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

        public void LoadSceneSync(string assetName)
        {
            var key = AssetSearchKey.Allocate(assetName);

            if (AssetFactory.assetBundleSceneAssetCreator.Match(key))
            {
                var asset = AssetFactory.assetBundleSceneAssetCreator.Create(key) as AssetBundleSceneAsset;
#if UNITY_EDITOR
                var simulate = true;
                if(simulate)
                {
                    string path = UnityEditor.AssetDatabase.GetAssetPathsFromAssetBundle(asset.assetBundleName)[0];
                    if(!string.IsNullOrEmpty(path))
                    {
                        UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path, new LoadSceneParameters());
                        key.Recycle();
                        m_TempDependencies.Clear();
                        return;
                    }
                }
// #else
                LoadAssetSync(key);
                SceneManager.LoadScene(assetName);
                key.Recycle();
                m_TempDependencies.Clear();
                return;
#endif
            }
            else
            {
                key.Recycle();
                m_TempDependencies.Clear();
                Logger.LogError("资源名称错误！请检查资源名称是否正确或是否被标记！assetname : " + assetName);
            }
        }

        public IAsset LoadAssetSync(AssetSearchKey key)
        {
            Add2Load(key);
            LoadSync();

            var asset = AssetManager.instance.GetAsset(key);
            if (asset == null)
            {

            }
            else
            {

            }
            return asset;
        }

        private void LoadSync()
        {
            while (m_WaitLoadList.Count > 0)
            {
                var first = m_WaitLoadList.First.Value;
                --m_LoadingCount;
                m_WaitLoadList.RemoveFirst();
                if (first == null) return;
                if (first.LoadSync())
                {

                }
            }
        }

        private Action m_Listener;
        public void LoadAsync(Action listener)
        {
            m_Listener = listener;
            DoLoadAsync();
            m_TempDependencies.Clear();
        }

        List<string> m_TempDependencies = new List<string>();
        private readonly List<IAsset> m_AssetList = new List<IAsset>();
        // 待加载的资源列表
        private readonly LinkedList<IAsset> m_WaitLoadList = new LinkedList<IAsset>();

        // 待加载以及正在加载的资源数量
        private int m_LoadingCount;

        public void Add2Load(string assetName, Action<bool, IAsset> listener = null, bool lastOrder = true)
        {
            var key = AssetSearchKey.Allocate(assetName);
            Add2Load(key, listener, lastOrder);
            key.Recycle();
        }

        public void Add2Load(string assetName, string assetBundleName, Action<bool, IAsset> listener = null, bool lastOrder = true)
        {
            var key = AssetSearchKey.Allocate(assetName, assetBundleName);
            Add2Load(key, listener, lastOrder);
            key.Recycle();
        }

        private void Add2Load(AssetSearchKey key, Action<bool, IAsset> listener = null, bool lastOrder = true)
        {
            IAsset asset = FindAssetInArray(m_AssetList, key);
            if (asset != null)
            {
                if (listener != null)
                {
                    // marktodo
                    asset.AddLoadDoneListener(listener);
                }
                return;
            }

            asset = AssetManager.instance.GetOrCreateAsset(key);

            if (asset == null)
            {
                return;
            }

            if (listener != null)
            {
                // marktodo
                asset.AddLoadDoneListener(listener);
            }

            var dependencies = asset.GetDependencies();
            if (dependencies != null)
            {
                foreach (var item in dependencies)
                {
                    if (!m_TempDependencies.Contains(item))
                    {
                        var searchKey = AssetSearchKey.Allocate(item, null, typeof(AssetBundle));
                        m_TempDependencies.Add(item);
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

            if (oldAsset != null)
            {
                return;
            }

            asset.Retain();
            m_AssetList.Add(asset);
            if (asset.state != AssetState.Ready)
            {
                ++m_LoadingCount;
                if (lastOrder)
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
            if (list == null) return null;
            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (key.Match(list[i]))
                {
                    return list[i];
                }
            }
            return null;
        }

        private void DoLoadAsync()
        {
            if (m_LoadingCount == 0)
            {
                if (m_Listener != null)
                {
                    var callback = m_Listener;
                    m_Listener = null;
                    callback();
                }
                return;
            }

            var nextNode = m_WaitLoadList.First;
            LinkedListNode<IAsset> currentNode = null;
            while (nextNode != null)
            {
                currentNode = nextNode;
                var asset = currentNode.Value;
                nextNode = currentNode.Next;
                if (asset.CheckDependenciesLoadDone())
                {
                    m_WaitLoadList.Remove(currentNode);
                    if (asset.state != AssetState.Ready)
                    {
                        asset.AddLoadDoneListener(OnAssetLoadDone);
                        asset.LoadAsync();
                    }
                    else
                    {
                        --m_LoadingCount;
                    }
                }
            }
        }

        private void OnAssetLoadDone(bool result, IAsset asset)
        {
            --m_LoadingCount;
            DoLoadAsync();  // 加载下一个资源
            if (m_LoadingCount == 0)
            {
                if (m_Listener != null)
                {
                    m_Listener();
                }
            }
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