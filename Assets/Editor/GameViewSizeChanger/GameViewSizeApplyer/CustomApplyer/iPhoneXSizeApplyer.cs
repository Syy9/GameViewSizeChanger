using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools
{
    public class iPhoneXSizeApplyer : GameViewSizeApplyer
    {
        GameViewOverlay overlay;
        Texture _portraitTexture;
        Texture PortraitTexture
        {
            get
            {
                return _portraitTexture = _portraitTexture ?? (Texture)EditorGUIUtility.Load("GameViewSizeChanger/iPhoneX_Portrait.png");
            }
        }

        Texture _landscapeTexture;
        Texture LandscapeTexture
        {
            get
            {
                return _landscapeTexture = _landscapeTexture ?? (Texture)EditorGUIUtility.Load("GameViewSizeChanger/iPhoneX_Landscape.png");
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
            GUI.DrawTexture(rect, orientation == Orientation.Portrait ? PortraitTexture : LandscapeTexture);
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
