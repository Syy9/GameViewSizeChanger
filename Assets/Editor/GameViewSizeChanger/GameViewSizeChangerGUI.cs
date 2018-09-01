using UnityEngine;

namespace Syy.GameViewSizeChanger
{
    public class GameViewSizeChangerGUI
    {
        IGameViewSizeData data;
        public GameViewSizeChangerGUI(IGameViewSizeData data)
        {
            this.data = data;
        }
        public bool OnGUI(bool isHighlight)
        {
            var defaultColor = GUI.color;
            if (isHighlight)
            {
                GUI.color = Color.gray;
            }
            if (GUILayout.Button(data.ToText(), "box", GUILayout.ExpandWidth(true)))
            {
                GUI.color = defaultColor;
                return true;
            }
            GUI.color = defaultColor;
            return false;
        }
    }
}
