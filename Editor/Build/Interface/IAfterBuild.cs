using UnityEditor;

namespace Litchi.Editor.Build
{
    public interface IAfterBuild
    {
        void OnAfterBuild(BuildTarget target, string locationPathName);
    }
}