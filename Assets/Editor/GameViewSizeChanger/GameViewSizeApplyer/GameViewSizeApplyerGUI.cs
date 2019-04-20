using UnityEngine;
using UnityEditor;
using System;

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

            bool isClick = false;
            using (new ColorScope(isHighlight ? Color.gray : GUI.color))
            {
                if (GUILayout.Button(_data.ToText(), _style, GUILayout.ExpandWidth(true)))
                {
                    isClick = true;
                }
            }

            return isClick;
        }

        class ColorScope : IDisposable
        {
            private Color _prevColor;
            public ColorScope(Color color)
            {
                _prevColor = GUI.color;
                GUI.color = color;
            }

            public void Dispose()
            {
                GUI.color = _prevColor;
            }
        }
    }
}
