using System;
using System.Collections;
using System.Collections.Generic;
using Kyusyukeigo.Helper;
using UnityEditor;
using UnityEngine;

namespace Syy.GameViewSizeChanger
{
    public class GameViewSizeChanger : EditorWindow
    {
        [MenuItem("Window/GameViewSizeChanger")]
        public static void Open()
        {
            GetWindow<GameViewSizeChanger>("GameViewSizeChanger");
        }

        private readonly GameViewSizeApplyer[] presets = new GameViewSizeApplyer[]
        {
            //iOS
            new GameViewSizeApplyer() {Title="iPhone4", Aspect="2:3", Width=640, Height=960, },
            new GameViewSizeApplyer() {Title="iPhone8", Aspect="9:16", Width=750, Height=1334, },
            new GameViewSizeApplyer() {Title="iPhoneX", Aspect="1:2", Width=1125, Height=2436, },
            new GameViewSizeApplyer() {Title="iPad", Aspect="3:4", Width=768, Height=1024, },
            // Android
            new GameViewSizeApplyer() {Title="GalaxyS8", Aspect="18.5：9", Width=1440, Height=2960, },
            
            //new SizeData() {Title="", Aspect="", Width=1, Height=1, },
        };

        Orientation orientation;
        int selectPresetIndex = 0;

        void OnGUI()
        {
            for (int i = 0; i < presets.Length; i++)
            {
                var preset = presets[i];
                var sizes = UnityStats.screenRes.Split('x');
                var w = float.Parse(sizes[0]);
                var h = float.Parse(sizes[1]);
                bool isCurrentGameViewSize = preset.Width == w && preset.Height == h;
                var defaultColor = GUI.color;
                if(isCurrentGameViewSize)
                {
                    GUI.color = isCurrentGameViewSize ? Color.gray : defaultColor;
                    selectPresetIndex = i;
                }
                if (GUILayout.Button(preset.GetLabel(), "box", GUILayout.ExpandWidth(true)))
                {
                    StartGameViewSizeProcess(preset);
                }
                GUI.color = defaultColor;
            }

            using(var check = new EditorGUI.ChangeCheckScope())
            {
                orientation = (Orientation)EditorGUILayout.EnumPopup("Orientation", orientation);
                if(check.changed)
                {
                    foreach (var preset in presets)
                    {
                        preset.orientation = orientation;
                    }
                }
            }

            var e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.UpArrow)
                {
                    selectPresetIndex = Mathf.Max(0, selectPresetIndex - 1);
                    StartGameViewSizeProcess(presets[selectPresetIndex]);
                    e.Use();
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    selectPresetIndex = Mathf.Min(presets.Length - 1, selectPresetIndex + 1);
                    StartGameViewSizeProcess(presets[selectPresetIndex]);
                    e.Use();
                }
            }
        }

        void StartGameViewSizeProcess(GameViewSizeApplyer preset)
        {
            ChangeGameViewSize(preset);
            EditorApplication.delayCall += () =>
            {
                //Wait gameView size change completed
                EditorApplication.delayCall += () =>
                {
                    UpdateGameViewSizeToMinScale();
                    Focus();
                };
            };
        }

        void UpdateGameViewSizeToMinScale()
        {
            var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var assembly = typeof(Editor).Assembly;
            var type = assembly.GetType("UnityEditor.GameView");
            EditorWindow gameView = EditorWindow.GetWindow(type);
            var minScaleProperty = type.GetProperty("minScale", flag);
            float minScale = (float) minScaleProperty.GetValue(gameView, null);
            type.GetMethod("SnapZoom", flag, null, new System.Type[] { typeof(float) }, null).Invoke(gameView, new object[] { minScale });
            EditorApplication.QueuePlayerLoopUpdate();
        }

        void ChangeGameViewSize(GameViewSizeApplyer data)
        {
            var gameViewSize = data.Convert();
            var groupType = GetCurrentGroupType();
            if(!GameViewSizeHelper.Contains(groupType, gameViewSize))
            {
                GameViewSizeHelper.AddCustomSize(groupType, gameViewSize);
            }
            GameViewSizeHelper.ChangeGameViewSize(GetCurrentGroupType(), gameViewSize);
        }

        GameViewSizeGroupType GetCurrentGroupType()
        {
            switch(EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                    return GameViewSizeGroupType.Android;
                case BuildTarget.iOS:
                    return GameViewSizeGroupType.iOS;
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                case BuildTarget.StandaloneOSX:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneWindows64:
                    return GameViewSizeGroupType.Standalone;
            }
            throw new NotImplementedException("Not Implemented BuildTargetType=" + EditorUserBuildSettings.activeBuildTarget.ToString());
        }
    }
}
