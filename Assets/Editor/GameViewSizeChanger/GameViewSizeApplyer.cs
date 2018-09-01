using System;
using System.Collections;
using System.Collections.Generic;
using Kyusyukeigo.Helper;
using UnityEditor;
using UnityEngine;
namespace Syy.GameViewSizeChanger
{
    public class GameViewSizeApplyer
    {
        public string Title;
        public string Aspect;
        public int Width;
        public int Height;
        public Orientation orientation;
        public event Action OnChangeGameViewSize;

        GameViewSizeChangerGUI gui;

        public string GetLabel()
        {
            bool isPortrait = orientation == Orientation.Portrait;
            string arrow = isPortrait ? "↑" : "→";
            int w = isPortrait ? Width : Height;
            int h = isPortrait ? Height : Width;
            return string.Format("【{0}】 {1} 【{2}={3}x{4}】", arrow, Title, Aspect, w, h);
        }

        public GameViewSizeApplyer()
        {
            gui = new GameViewSizeChangerGUI(this);
        }

        public GameViewSizeHelper.GameViewSize Convert()
        {
            var gameViewSize = new GameViewSizeHelper.GameViewSize();
            gameViewSize.type = GameViewSizeHelper.GameViewSizeType.FixedResolution;
            gameViewSize.baseText = GetLabel();
            bool isPortrait = orientation == Orientation.Portrait;
            int w = isPortrait ? Width : Height;
            int h = isPortrait ? Height : Width;
            gameViewSize.width = w;
            gameViewSize.height = h;
            return gameViewSize;
        }

        public void OnGUI()
        {
            bool isHighlight = IsCurrentGameViewSize();
            if(gui.OnGUI(isHighlight))
            {
                Apply();
            }
        }

        public bool IsCurrentGameViewSize()
        {
            var sizes = UnityStats.screenRes.Split('x');
            var w = float.Parse(sizes[0]);
            var h = float.Parse(sizes[1]);
            return Width == w && Height == h;
        }

        public void Apply()
        {
            ApplyImpl();
            EditorApplication.delayCall += () =>
            {
                //Wait gameView size change completed
                EditorApplication.delayCall += () =>
                {
                    UpdateGameViewSizeToMinScale();
                    if(OnChangeGameViewSize != null)
                    {
                        OnChangeGameViewSize();
                    }
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
            float minScale = (float)minScaleProperty.GetValue(gameView, null);
            type.GetMethod("SnapZoom", flag, null, new System.Type[] { typeof(float) }, null).Invoke(gameView, new object[] { minScale });
            EditorApplication.QueuePlayerLoopUpdate();
        }

        void ApplyImpl()
        {
            var gameViewSize = Convert();
            var groupType = GetCurrentGroupType();
            if (!GameViewSizeHelper.Contains(groupType, gameViewSize))
            {
                GameViewSizeHelper.AddCustomSize(groupType, gameViewSize);
            }
            GameViewSizeHelper.ChangeGameViewSize(GetCurrentGroupType(), gameViewSize);
        }

        GameViewSizeGroupType GetCurrentGroupType()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
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

    public enum Orientation
    {
        Portrait, //↑
        Landscape, //→
    }
}

