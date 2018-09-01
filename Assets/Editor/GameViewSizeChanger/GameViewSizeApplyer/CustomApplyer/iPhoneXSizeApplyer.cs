using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace Syy.GameViewSizeChanger
{
    public class iPhoneXSizeApplyer : GameViewSizeApplyer
    {
        GameViewOverlay overlay;
        Texture _portraitTexture;
        Texture PortraitTexture {
            get {
                if(_portraitTexture == null)
                {
                    _portraitTexture = (Texture)EditorGUIUtility.Load("GameViewSizeChanger/iPhoneX_Portrait.png");
                }
                return _portraitTexture;
            }
        }

        Texture _landscapeTexture;
        Texture LandscapeTexture
        {
            get
            {
                if (_landscapeTexture == null)
                {
                    _landscapeTexture = (Texture)EditorGUIUtility.Load("GameViewSizeChanger/iPhoneX_Landscape.png");
                }
                return _landscapeTexture;
            }
        }
        public override void Apply()
        {
            base.Apply();
            Clear();
            var overlayObj = new GameObject();
            overlayObj.hideFlags = HideFlags.DontSave | HideFlags.HideInHierarchy;
            overlay = overlayObj.AddComponent<GameViewOverlay>();
            overlay.OnGUIEvent += DrawOvalay;
        }

        public override void NoticeChangedOtherSize()
        {
            base.NoticeChangedOtherSize();
            Clear();
        }

        void DrawOvalay()
        {
            var rect = new Rect(0, 0, Screen.width, Screen.height);
            if(orientation == Orientation.Portrait)
            {
                GUI.DrawTexture(rect, PortraitTexture);
            } else {
                GUI.DrawTexture(rect, LandscapeTexture);
            }
            
        }

        void Clear()
        {
            if (overlay != null)
            {
                overlay.OnGUIEvent -= DrawOvalay;
                GameObject.DestroyImmediate(overlay.gameObject);
            }
        }


        [ExecuteInEditMode()]
        [AddComponentMenu("")]
        public class GameViewOverlay : MonoBehaviour
        {
            public event Action OnGUIEvent;

            void OnGUI()
            {
                if (OnGUIEvent != null)
                {
                    OnGUIEvent();
                }
            }
        }

    }

}
