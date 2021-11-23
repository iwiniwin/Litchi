using UnityEngine;
using UnityEditor;

namespace Litchi.Editor
{
    public abstract class SettingsWindow : EditorWindow
    {
        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            Draw();
            EditorGUILayout.EndScrollView();
        }

        private void OnDestroy()
        {
            Save();
        }
        protected abstract void Save();
        protected abstract void Draw();
    }

    public class AssetManageSettingsWindow : SettingsWindow
    {
        [MenuItem("Litchi/Asset Manage", priority = 5)]
        public static void ShowWindow()
        {
            GetWindow<AssetManageSettingsWindow>("Asset Manage Settings");
        }

        protected override void Draw()
        {
            SettingsDrawer.DrawObjectFields(Litchi.Config.Settings.assetManage);
        }
        
        
        protected override void Save()
        {
            
        }
    }

    public class HotfixWindow : SettingsWindow
    {
        [MenuItem("Litchi/Hotfix", priority = 6)]
        public static void ShowWindow()
        {
            GetWindow<HotfixWindow>("Hotfix Settings");
        }

        protected override void Draw()
        {
            SettingsDrawer.DrawObjectFields(Litchi.Config.Settings.assetManage);
        }
        
        
        protected override void Save()
        {
            
        }
    }
}