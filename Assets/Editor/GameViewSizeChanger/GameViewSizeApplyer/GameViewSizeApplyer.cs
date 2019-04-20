using System;
using System.Collections;
using System.Collections.Generic;
using Kyusyukeigo.Helper;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools
{
    public class GameViewSizeApplyer : IGameViewSizeData, IGameViewSizeApplyer
    {
        public string Title { get; set; }
        public string Aspect { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Orientation orientation { get; set; }
        public event Action OnChangeGameViewSize;

        GameViewSizeApplyerGUI _gui;

        public GameViewSizeApplyer()
        {
            _gui = new GameViewSizeApplyerGUI(this);
        }

        public void OnGUI()
        {
            bool isHighlight = IsSelect();
            if (_gui.OnGUI(isHighlight))
            {
                Apply();
            }
        }

        public virtual void Apply()
        {
            ApplyImpl();
            EditorApplication.QueuePlayerLoopUpdate();
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

        public bool IsSelect()
        {
            var sizes = UnityStats.screenRes.Split('x');
            int w = 0;
            int h = 0;
            if(orientation == Orientation.Portrait)
            {
                w = int.Parse(sizes[0]);
                h = int.Parse(sizes[1]);
            }
            else
            {
                w = int.Parse(sizes[1]);
                h = int.Parse(sizes[0]);
            }

            return Width == w && Height == h;
        }

        public virtual void NoticeChangedOtherSize()
        {
        }

        public string ToText()
        {
            bool isPortrait = orientation == Orientation.Portrait;
            string arrow = isPortrait ? "↑" : "→";
            int w = isPortrait ? Width : Height;
            int h = isPortrait ? Height : Width;
            return string.Format("【{0}】 {1} 【{2}={3}x{4}】", arrow, Title, Aspect, w, h);
        }

        GameViewSizeHelper.GameViewSize Convert()
        {
            var gameViewSize = new GameViewSizeHelper.GameViewSize();
            gameViewSize.type = GameViewSizeHelper.GameViewSizeType.FixedResolution;
            gameViewSize.baseText = ToText();
            bool isPortrait = orientation == Orientation.Portrait;
            int w = isPortrait ? Width : Height;
            int h = isPortrait ? Height : Width;
            gameViewSize.width = w;
            gameViewSize.height = h;
            return gameViewSize;
        }

        GameViewSizeGroupType GetCurrentGroupType()
        {
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android: return GameViewSizeGroupType.Android;
                case BuildTarget.iOS: return GameViewSizeGroupType.iOS;
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

        void ApplyImpl()
        {
            var gameViewSize = Convert();
            var groupType = GetCurrentGroupType();
            if (!GameViewSizeHelper.Contains(groupType, gameViewSize))
            {
                GameViewSizeHelper.AddCustomSize(groupType, gameViewSize);
            }
            GameViewSizeHelper.ChangeGameViewSize(groupType, gameViewSize);
        }

        void UpdateGameViewSizeToMinScale()
        {
            var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var assembly = typeof(Editor).Assembly;
            var type = assembly.GetType("UnityEditor.GameView");
            var gameView = EditorWindow.GetWindow(type);
            var minScaleProperty = type.GetProperty("minScale", flag);
            float minScale = (float)minScaleProperty.GetValue(gameView, null);
            type.GetMethod("SnapZoom", flag, null, new System.Type[] { typeof(float) }, null).Invoke(gameView, new object[] { minScale });
        }
    }

    public enum Orientation
    {
        Portrait, //↑
        Landscape, //→
    }
}
