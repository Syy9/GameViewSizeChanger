using UnityEngine;
using UnityEditor;

namespace Syy.Tools
{
    public class GameViewSizeApplyerGUI
    {
        IGameViewSizeData _data;
        public GameViewSizeApplyerGUI(IGameViewSizeData data)
        {
            _data = data;
        }

        public bool OnGUI(bool isHighlight)
        {
            var defaultColor = GUI.color;
            if (isHighlight)
            {
                GUI.color = Color.gray;
            }

            var style = new GUIStyle(GUI.skin.box);
            style.normal.textColor = EditorStyles.toolbar.normal.textColor;
            if (GUILayout.Button(_data.ToText(), style, GUILayout.ExpandWidth(true)))
            {
                GUI.color = defaultColor;
                return true;
            }
            GUI.color = defaultColor;
            return false;
        }
    }
}
