using UnityEditor;

namespace Litchi.Editor.Build
{
    public interface IModifyPlayerSettings
    {
        void OnModifyPlayerSettings(BuildTarget target);
    }
}