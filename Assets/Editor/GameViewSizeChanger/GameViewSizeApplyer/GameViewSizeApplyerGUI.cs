using UnityEngine;
using UnityEditor;

namespace Syy.Tools
{
    public class GameViewSizeApplyerGUI
    {
        IGameViewSizeData _data;
        GUIStyle _style;

        public GameViewSizeApplyerGUI(IGameViewSizeData data)
        {
            _data = data;
        }

        public bool OnGUI(bool isHighlight)
        {
            if (_style == null)
            {
                _style = new GUIStyle(GUI.skin.box);
                _style.normal.textColor = EditorStyles.toolbar.normal.textColor;
            }

            var defaultColor = GUI.color;
            if (isHighlight)
            {
                GUI.color = Color.gray;
            }

            if (GUILayout.Button(_data.ToText(), _style, GUILayout.ExpandWidth(true)))
            {
                GUI.color = defaultColor;
                return true;
            }
            GUI.color = defaultColor;
            return false;
        }
    }
}
