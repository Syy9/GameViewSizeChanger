using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools.GameViewSizeTool
{
    public class DrawTextureResizeTool : ResizeTool
    {
        OnGUIEventHandler _handler = null;
        Texture PortraitTexture;
        Texture LandscapeTexture;

        public DrawTextureResizeTool(GameViewSizeChanger window, string label, int width, int height, string portraitTextureGuid, string landscapeTextureGuid) : base(window, label, width, height)
        {
            PortraitTexture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(portraitTextureGuid));
            LandscapeTexture = AssetDatabase.LoadAssetAtPath<Texture>(AssetDatabase.GUIDToAssetPath(landscapeTextureGuid));
        }

        public override void Dispose()
        {
            if (_handler != null && _handler.gameObject != null)
            {
                _handler.OnGUIEvent -= OnDrawTexture;
                GameObject.DestroyImmediate(_handler.gameObject);
            }
        }

        public override void Resize()
        {
            ResizeImple();
            if (_handler == null)
            {
                var go = new GameObject();
                go.hideFlags = HideFlags.DontSave;
                go.name = "OnGUIEventHandler (DontSaveFlag)";
                _handler = go.AddComponent<OnGUIEventHandler>();
                _handler.OnGUIEvent += OnDrawTexture;
            }
        }

        void OnDrawTexture()
        {
            if (PortraitTexture != null && LandscapeTexture != null)
            {
                var rect = new Rect(0, 0, Screen.width, Screen.height);
                GUI.DrawTexture(rect, Window.Orientation == Orientation.Portrait ? PortraitTexture : LandscapeTexture);
            }
        }

        [ExecuteInEditMode()]
        [AddComponentMenu("")]
        public class OnGUIEventHandler : MonoBehaviour
        {
            public event Action OnGUIEvent;

            void OnGUI()
            {
                OnGUIEvent?.Invoke();
            }
        }
    }
}
