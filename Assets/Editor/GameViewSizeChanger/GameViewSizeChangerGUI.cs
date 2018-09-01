using UnityEngine;

namespace Syy.GameViewSizeChanger
{
    public class GameViewSizeChangerGUI
    {
        GameViewSizeApplyer applyer;
        public GameViewSizeChangerGUI(GameViewSizeApplyer applyer)
        {
            this.applyer = applyer;
        }
        public bool OnGUI(bool isHighlight)
        {
            var defaultColor = GUI.color;
            if (isHighlight)
            {
                GUI.color = Color.gray;
            }
            if (GUILayout.Button(applyer.GetLabel(), "box", GUILayout.ExpandWidth(true)))
            {
                GUI.color = defaultColor;
                return true;
            }
            GUI.color = defaultColor;
            return false;
        }
    }
}
