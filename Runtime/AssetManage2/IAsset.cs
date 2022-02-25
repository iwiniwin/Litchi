using System;

namespace Litchi.AssetManage2
{
    public enum AssetState 
    {
        Waiting = 0,
        Loading = 1,
        Ready = 2
    }

    public interface IAsset : IRefCounter, IEnumeratorTask
    {
        string assetName { get; }
        string assetBundleName { get; }
        AssetState state { get; }
        Type assetType { get; } 
        UnityEngine.Object asset { get; }

        float progress { get; }

        void AddLoadDoneListener(Action<bool, IAsset> listener);
        void RemoveLoadDoneListener(Action<bool, IAsset> listener);

        bool UnloadImage(bool flag);

        bool LoadSync();

        void LoadAsync();

        string[] GetDependencies();

        bool CheckDependenciesLoadDone();

        bool ReleaseAsset();

        void Recycle();
    }
}