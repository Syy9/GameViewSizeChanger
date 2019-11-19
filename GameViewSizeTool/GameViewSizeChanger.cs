using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools.GameViewSizeTool
{
    public class GameViewSizeChanger : EditorWindow
    {
        public const string Key_GameViewSizeChanger_Orientation = "Key_GameViewSizeChanger_Orientation";
        public const string Key_GameViewSizeChanger_LastLabel = "Key_GameViewSizeChanger_LastLabel";

        [MenuItem("Window/GameViewSizeChanger")]
        public static void Open()
        {
            GameViewSizeChanger window = GetWindow<GameViewSizeChanger>("GameViewSizeChanger");
            window.Init(useResize: true);
        }

        ResizeTool[] _resizeTools;
        ResizeTool _activeTool;

        [SerializeField]
        Orientation _orientation;
        public Orientation Orientation => _orientation;

        void OnEnable()
        {
            Init(useResize: false);
        }

        void OnDisable()
        {
            foreach (var tool in _resizeTools)
            {
                tool.Dispose();
            }
        }

        void Init(bool useResize)
        {
            _resizeTools = new ResizeTool[] {
                //iOS
                new ResizeTool(this, "iPhone4", 640, 960),
                new ResizeTool(this, "iPhone8", 750, 1334),
                new DrawTextureResizeTool(this, "iPhoneX", 1125, 2436, "13879482f80b44c4e8c46d6a421f03bd", "11f0a686942544a7eb63960d9546e970"),
                new ResizeTool(this, "iPad", 768, 1024),
                // Android
                new ResizeTool(this, "GalaxyS8", 1440, 2960),
            };

            if (useResize)
            {
                if (Save.ContainsKey(Key_GameViewSizeChanger_Orientation))
                {
                    _orientation = Save.Get<Orientation>(Key_GameViewSizeChanger_Orientation);
                }

                // 前回使ったGameViewSizeがあれば適用する
                if (Save.ContainsKey(Key_GameViewSizeChanger_LastLabel))
                {
                    if (!EditorApplication.isPlaying && !EditorApplication.isCompiling && !EditorApplication.isPlayingOrWillChangePlaymode)
                    {
                        var label = Save.Get<string>(Key_GameViewSizeChanger_LastLabel);
                        var tool = _resizeTools.FirstOrDefault(value => value.Label == label);
                        tool?.Resize();
                        _activeTool = tool;
                    }
                }
            }
        }

        void OnGUI()
        {
            using (var check = new EditorGUI.ChangeCheckScope())
            {
                EditorGUILayout.LabelField("Orientation");
                using (new EditorGUILayout.HorizontalScope())
                {
                    DrawOeientationButton(Orientation.Portrait, EditorStyles.miniButtonLeft);
                    DrawOeientationButton(Orientation.Landscape, EditorStyles.miniButtonRight);
                }
                if (check.changed)
                {
                    Save.Set<Orientation>(Key_GameViewSizeChanger_Orientation, _orientation);
                }
            }

            EditorGUILayout.LabelField("GameViewSize");
            foreach (var tool in _resizeTools)
            {
                if (tool.OnGUI())
                {
                    if (_activeTool != tool)
                    {
                        _activeTool?.Dispose();
                        _activeTool = tool;
                    }
                }
            }
        }

        void DrawOeientationButton(Orientation orientation, GUIStyle style)
        {
            string mark = _orientation == orientation ? "◆" : "";
            if (GUILayout.Button(mark + orientation.ToString() + mark, style))
            {
                _orientation = orientation;
                _activeTool?.Resize();
            }
        }
    }

    public static class Save
    {
        public static bool ContainsKey(string key)
        {
            return !string.IsNullOrEmpty(EditorUserSettings.GetConfigValue(key));
        }

        public static T Get<T>(string key)
        {
            string value = EditorUserSettings.GetConfigValue(key);
            Type type = typeof(T);
            if (type.IsEnum)
            {
                return (T)Enum.Parse(type, value);
            }
            if (type == typeof(int))
            {
                return (T)(object)int.Parse(value);
            }
            return (T)(object)value;
        }

        public static void Set<T>(string key, T value)
        {
            EditorUserSettings.SetConfigValue(key, value.ToString());
        }
    }

    public enum Orientation
    {
        Portrait = 0,
        Landscape = 1,
    }
}
