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
            GetWindow<GameViewSizeChanger>();
        }

        private static readonly SizeData[] presets = new SizeData[]
        {
            //iOS
            new SizeData() {Title="iPhoneX", Aspect="1:2", Width=1125, Height=2436, },
            new SizeData() {Title="iPhone8", Aspect="9:16", Width=750, Height=1334, },
            new SizeData() {Title="iPhone4", Aspect="2:3", Width=640, Height=960, },
            new SizeData() {Title="iPad", Aspect="3:4", Width=768, Height=1024, },
            // Android
            new SizeData() {Title="GalaxyS8", Aspect="18.5：9", Width=1440, Height=2960, },
            
            //new SizeData() {Title="", Aspect="", Width=1, Height=1, },
        };

        static Orientation orientation;

        void OnGUI()
        {
            orientation = (Orientation)EditorGUILayout.EnumPopup("Orientation", orientation);
            foreach (var preset in presets)
            {
                if (GUILayout.Button(preset.GetLabel()))
                {
                    ChangeGameViewSize(preset);
                }
            }
        }

        void ChangeGameViewSize(SizeData data)
        {
            var gameViewSize = data.Convert();
            var groupType = GetCurrentGroupType();
            if(!GameViewSizeHelper.Contains(groupType, gameViewSize))
            {
                if(EditorUtility.DisplayDialog("Register new GameViewSize?", data.GetLabel(), "OK", "Cancel"))
                {
                    GameViewSizeHelper.AddCustomSize(groupType, gameViewSize);
                } else {
                    return;
                }
            }
            GameViewSizeHelper.ChangeGameViewSize(GetCurrentGroupType(), gameViewSize);
            Debug.Log("Changed GameViewSize! " + data.GetLabel());
        }

        GameViewSizeGroupType GetCurrentGroupType()
        {
            GameViewSizeGroupType type = GameViewSizeGroupType.Android;
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

        private class SizeData {
            public string Title;
            public string Aspect;
            public int Width;
            public int Height;

            public string GetLabel()
            {
                bool isPortrait = orientation == Orientation.Portrait;
                string arrow = isPortrait ? "↑" : "→";
                int w = isPortrait ? Width : Height;
                int h = isPortrait ? Height : Width;
                return string.Format("【{0}】 {1} 【{2}={3}x{4}】", arrow, Title, Aspect, w, h);
            }

            public GameViewSizeHelper.GameViewSize Convert()
            {
                var gameViewSize = new GameViewSizeHelper.GameViewSize();
                gameViewSize.type = GameViewSizeHelper.GameViewSizeType.AspectRatio;
                gameViewSize.baseText = GetLabel();
                bool isPortrait = orientation == Orientation.Portrait;
                int w = isPortrait ? Width : Height;
                int h = isPortrait ? Height : Width;
                gameViewSize.width = w;
                gameViewSize.height = h;
                return gameViewSize;
            }
        }

        public enum Orientation
        {
            Portrait, //↑
            Landscape, //→
        }
    }
}
