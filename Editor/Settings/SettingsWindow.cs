using UnityEngine;
using UnityEditor;

namespace Litchi.Editor
{
    public class SettingsWindow : EditorWindow
    {
        [MenuItem("Litchi/Test", priority = 5)]
        public static void ShowWindow()
        {
            GetWindow<SettingsWindow>("SettingsWindow Ws");
        }
        
        private Vector2 scrollPos = Vector2.zero;
        private void OnGUI()
        {
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            SettingsDrawer.DrawObjectFields(Litchi.Config.Settings.assetManage);
            
            EditorGUILayout.EndScrollView();
        }

    }
}