using UnityEditor;

namespace Litchi.Editor.Build
{
    public interface IFinallyBuild
    {
        void OnFinallyBuild(BuildTarget target, string locationPathName);
    }
}