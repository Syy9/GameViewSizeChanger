using System;
using System.Collections;
using System.Collections.Generic;
using Kyusyukeigo.Helper;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools
{
    public class GameViewSizeChanger : EditorWindow
    {
        [MenuItem("Window/GameViewSizeChanger")]
        public static void Open()
        {
            GetWindow<GameViewSizeChanger>("GameViewSizeChanger");
        }

        static readonly GameViewSizeApplyer[] Applyers = new GameViewSizeApplyer[]
        {
            //iOS
            new GameViewSizeApplyer() { Title="iPhone4", Aspect="2:3", Width=640, Height=960, },
            new GameViewSizeApplyer() { Title="iPhone8", Aspect="9:16", Width=750, Height=1334, },
            new iPhoneXSizeApplyer () { Title="iPhoneX", Aspect="1:2", Width=1125, Height=2436, },
            new GameViewSizeApplyer() { Title="iPad"   , Aspect="3:4", Width=768, Height=1024, },
            // Android
            new GameViewSizeApplyer() { Title="GalaxyS8", Aspect="18.5：9", Width=1440, Height=2960, },

            //new SizeData() {Title="", Aspect="", Width=1, Height=1, },
        };

        Orientation _orientation = Orientation.Portrait;
        int _selectIndex = 0;

        void OnEnable()
        {
            int index = 0;
            foreach (var applyer in Applyers)
            {
                applyer.orientation = _orientation;
                applyer.OnChangeGameViewSize += OnChangeGameViewSize;
                if (applyer.IsSelect())
                {
                    _selectIndex = index;
                }
                index++;
            }
        }

        void OnDisable()
        {
            foreach (var applyer in Applyers)
            {
                applyer.OnChangeGameViewSize -= OnChangeGameViewSize;
            }
        }

        void OnGUI()
        {
            foreach (var applyer in Applyers)
            {
                applyer.OnGUI();
            }

            using (var check = new EditorGUI.ChangeCheckScope())
            {
                _orientation = (Orientation)EditorGUILayout.EnumPopup("Orientation", _orientation);
                if (check.changed)
                {
                    foreach (var applyer in Applyers)
                    {
                        applyer.orientation = _orientation;
                    }
                }
            }

            var e = Event.current;
            if (e.type == EventType.KeyDown)
            {
                if (e.keyCode == KeyCode.UpArrow)
                {
                    _selectIndex--;
                    if(_selectIndex < 0)
                    {
                        _selectIndex = Applyers.Length -1;
                    }
                    Applyers[_selectIndex].Apply();
                    e.Use();
                }
                else if (e.keyCode == KeyCode.DownArrow)
                {
                    _selectIndex++;
                    if (_selectIndex > (Applyers.Length - 1))
                    {
                        _selectIndex = 0;
                    }
                    Applyers[_selectIndex].Apply();
                    e.Use();
                }
            }
        }

        void OnChangeGameViewSize()
        {
            Repaint();
            Focus();
            int index = 0;
            foreach (var applyer in Applyers)
            {
                if (applyer.IsSelect())
                {
                    _selectIndex = index;
                }
                else
                {
                    applyer.NoticeChangedOtherSize();
                }
                index++;
            }
        }
    }
}
