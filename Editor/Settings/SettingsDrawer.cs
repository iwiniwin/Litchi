using UnityEditor;
using System.Reflection;
using System;

namespace Litchi.Editor
{
    public class SettingsDrawer
    {
        public static void DrawObjectFields(object o)
        {
            FieldInfo[] fields = o.GetType().GetFields();
            foreach(var f in fields)
            {
                if(f.FieldType == typeof(string))
                {
                    DrawStringField(o, f);
                }
                else if(f.FieldType == typeof(int))
                {
                    DrawIntField(o, f);
                }
                else if(f.FieldType.IsEnum)
                {
                    DrawEnumField(o, f);
                }
            }
        }

        public static void DrawIntField(object o, FieldInfo f)
        {
            DrawField(o, f, (obj, info) => info.SetValue(obj, EditorGUILayout.IntField((int)info.GetValue(obj))));
        }

        public static void DrawStringField(object o, FieldInfo f)
        {
            DrawField(o, f, (obj, info) => info.SetValue(obj, EditorGUILayout.TextField((string)info.GetValue(obj))));
        }

        public static void DrawEnumField(object o, FieldInfo f)
        {
            DrawField(o, f, (obj, info) => info.SetValue(obj, EditorGUILayout.EnumPopup((System.Enum)info.GetValue(obj))));
        }

        public static void DrawField(object obj, FieldInfo info, Action<object, FieldInfo> ac)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(info.Name);
            ac(obj, info);
            EditorGUILayout.EndHorizontal();
        }
    }
}