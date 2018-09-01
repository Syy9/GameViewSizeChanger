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

        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }

            var cast = (GameViewSizeApplyer)obj;
            return this.Title == cast.Title
                && this.Aspect == cast.Aspect
                && this.Width == cast.Width
                && this.Height == cast.Height;
        }

        public override int GetHashCode()
        {
            return int.Parse(this.Width.ToString() + "000" + this.Height.ToString());
        }
    }

    public enum Orientation
    {
        Portrait, //↑
        Landscape, //→
    }
}

