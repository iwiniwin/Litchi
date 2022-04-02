using UnityEditor;

namespace Litchi.Editor.Build
{
    public interface IBeforeBuild
    {
        void OnBeforeBuild(BuildTarget target);
    }
}