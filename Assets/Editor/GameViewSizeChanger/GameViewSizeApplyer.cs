using System.Collections;
using System.Collections.Generic;
using Kyusyukeigo.Helper;
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
            gameViewSize.type = GameViewSizeHelper.GameViewSizeType.FixedResolution;
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

