using Kyusyukeigo.Helper;
using UnityEditor;
using UnityEngine;

namespace Syy.Tools.GameViewSizeTool
{
    public class ResizeTool
    {
        protected GameViewSizeChanger Window;
        public string Label { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public ResizeTool(GameViewSizeChanger window, string label, int width, int height)
        {
            Window = window;
            Label = label;
            Width = width;
            Height = height;
        }

        public virtual void Dispose() { }

        public bool OnGUI()
        {
            string mark = IsSameScreenRes() ? "◆" : "";
            if (GUILayout.Button(mark + Label + mark))
            {
                Resize();
                return true;
            }

            return false;
        }

        public virtual void Resize()
        {
            ResizeImple();
        }

        protected void ResizeImple()
        {
            var groupType = GameViewSizeHelper.GetCurrentGameViewSizeGroupType();
            var gameViewSize = new GameViewSizeHelper.GameViewSize();
            gameViewSize.type = GameViewSizeHelper.GameViewSizeType.FixedResolution;
            gameViewSize.width = Window.Orientation == Orientation.Portrait ? Width : Height;
            gameViewSize.height = Window.Orientation == Orientation.Portrait ? Height : Width;
            gameViewSize.baseText = Label;

            if (!GameViewSizeHelper.Contains(groupType, gameViewSize))
            {
                GameViewSizeHelper.AddCustomSize(groupType, gameViewSize);
            }

            GameViewSizeHelper.ChangeGameViewSize(groupType, gameViewSize);
            Save.Set<string>(GameViewSizeChanger.Key_GameViewSizeChanger_LastLabel, Label);

            // HACK: 2フレーム待たないとSnapZoomが動作しないケースがある
            EditorApplication.delayCall += () =>
            {
                EditorApplication.delayCall += () =>
                {
                    var flag = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
                    var assembly = typeof(Editor).Assembly;
                    var type = assembly.GetType("UnityEditor.GameView");
                    var gameView = EditorWindow.GetWindow(type, false, "Game", false);
                    var minScaleProperty = type.GetProperty("minScale", flag);
                    float minScale = (float)minScaleProperty.GetValue(gameView, null);
                    type.GetMethod("SnapZoom", flag, null, new System.Type[] { typeof(float) }, null).Invoke(gameView, new object[] { minScale });
                    gameView.Repaint();
                    Window.Focus();
                };
            };
        }

        /// <summary>
        /// GameViewの画面サイズと同じか
        /// </summary>
        /// <returns></returns>
        public bool IsSameScreenRes()
        {
            var sizes = UnityStats.screenRes.Split('x');
            int w = 0;
            int h = 0;
            if (Window.Orientation == Orientation.Portrait)
            {
                w = int.Parse(sizes[0]);
                h = int.Parse(sizes[1]);
            }
            else
            {
                w = int.Parse(sizes[1]);
                h = int.Parse(sizes[0]);
            }

            return Width == w && Height == h;
        }
    }
}
