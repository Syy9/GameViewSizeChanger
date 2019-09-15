using System.Collections;
using System.Collections.Generic;
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
            GetWindow<GameViewSizeChanger>("GameViewSizeChanger");
        }

        ResizeTool[] _resizeTools;
        ResizeTool _activeTool;

        void OnEnable()
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

            if (EditorPrefs.HasKey(Key_GameViewSizeChanger_Orientation))
            {
                _orientation = (Orientation)EditorPrefs.GetInt(Key_GameViewSizeChanger_Orientation);
            }

            // 前回使ったGameViewSizeがあれば適用する
            if (EditorPrefs.HasKey(Key_GameViewSizeChanger_LastLabel))
            {
                var label = EditorPrefs.GetString(Key_GameViewSizeChanger_LastLabel);
                var tool = _resizeTools.FirstOrDefault(value => value.Label == label);
                tool?.Resize();
                _activeTool = tool;
                // Resize()するとこのwindowがUnityEditorの背面に回ってしまうためForcusする
                Focus();
            }
        }

        void OnDisable()
        {
            foreach (var tool in _resizeTools)
            {
                tool.Dispose();
            }
        }

        [SerializeField]
        Orientation _orientation;
        public Orientation Orientation => _orientation;

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
                    EditorPrefs.SetInt(Key_GameViewSizeChanger_Orientation, (int)_orientation);
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

    public enum Orientation
    {
        Portrait = 0,
        Landscape = 1,
    }
}
