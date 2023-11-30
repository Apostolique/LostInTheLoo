using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Apos.Input;
using Apos.Camera;
using Apos.Tweens;

namespace GameProject {
    public class GameRoot : Game {
        public GameRoot() {
            _graphics = new GraphicsDeviceManager(this) {
                GraphicsProfile = GraphicsProfile.HiDef
            };
            IsMouseVisible = true;
            Content.RootDirectory = "Content";

            Assets.LoadJson();
        }

        protected override void Initialize() {
            Utility.Game = this;
            Utility.Graphics = _graphics;

            Window.AllowUserResizing = true;

            IsFixedTimeStep = Assets.Settings.IsFixedTimeStep;
            _graphics.SynchronizeWithVerticalRetrace = Assets.Settings.IsVSync;

            Assets.Settings.IsFullscreen = Assets.Settings.IsFullscreen || Assets.Settings.IsBorderless;

            Utility.RestoreWindow();
            if (Assets.Settings.IsFullscreen) {
                Utility.ApplyFullscreenChange(false);
            }

            base.Initialize();
        }

        protected override void LoadContent() {
            InputHelper.Setup(this);

            Assets.LoadAssets(Content, GraphicsDevice);

            Window.ClientSizeChanged += ClientSizeChanged;
            ClientSizeChanged(null, null);

            _s = new SpriteBatch(GraphicsDevice);
            _sb = new ShapeBatch(GraphicsDevice);

            _camera = new Camera(new DensityViewport(GraphicsDevice, Window, 2000f, 2000f));
        }

        protected void ClientSizeChanged(object sender, EventArgs e) {
            int w = Window.ClientBounds.Width;
            int h = Window.ClientBounds.Height;
            if (w < 1) {
                w = 1;
            }
            if (h < 1) {
                h = 1;
            }
            Width = w;
            Height = h;

            _target1?.Dispose();
            _target2?.Dispose();
            CreateTargets();
        }

        protected override void UnloadContent() {
            if (!Assets.Settings.IsFullscreen) {
                Utility.SaveWindow();
            }

            Utility.SaveJson("Settings.json", Assets.Settings, SettingsContext.Default.Settings);

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime) {
            InputHelper.UpdateSetup();
            TweenHelper.UpdateSetup(gameTime);

            if (_quit.Pressed())
                Exit();

            if (_toggleFullscreen.Pressed()) {
                Utility.ToggleFullscreen();
            }
            if (_toggleBorderless.Pressed()) {
                Utility.ToggleBorderless();
            }

            UpdateCamera();

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.SetRenderTarget(_target2);
            GraphicsDevice.Clear(TWColor.Transparent);
            GraphicsDevice.SetRenderTarget(_target1);
            GraphicsDevice.Clear(TWColor.Transparent);

            _sb.Begin(view: _camera.GetView(-0.2f));
            _sb.DrawCircle(new Vector2(100f, 100f), 20f, TWColor.Red500, TWColor.White, 2f);
            _sb.End();
            PreserveRender(10f, -0.2f);

            _sb.Begin(view: _camera.GetView(-0.1f));
            _sb.DrawCircle(new Vector2(0f, 0f), 20f, TWColor.Blue200, TWColor.Black, 1f);
            _sb.End();
            PreserveRender(2f, -0.1f);

            _sb.Begin(view: _camera.GetView(0f));
            _sb.DrawEllipse(new Vector2(-100f, 50f), 50f, 20f, TWColor.Pink300, TWColor.Gray800, 1f);
            _sb.End();
            PreserveRender(0f, 0f);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(TWColor.Black);

            _s.Begin(transformMatrix: _camera.GetView(-3f));
            _s.Draw(Assets.Background, new Rectangle(-15000, -10000, Assets.Background.Width * 10, Assets.Background.Height * 10), TWColor.White);
            _s.End();

            _s.Begin();
            _s.Draw(_target2, Vector2.Zero, TWColor.White);
            _s.End();

            base.Draw(gameTime);
        }

        private void PreserveRender(float blurRadius, float z) {
            GraphicsDevice.SetRenderTarget(_target2);
            Assets.Bokeh.Parameters["r"].SetValue(blurRadius * _camera.WorldToScreenScale(z));
            _s.Begin(effect: Assets.Bokeh);
            _s.Draw(_target1, Vector2.Zero, TWColor.White);
            _s.End();
            GraphicsDevice.SetRenderTarget(_target1);
            GraphicsDevice.Clear(TWColor.Transparent);
        }

        private void CreateTargets() {
            _target1 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _target2 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Assets.Bokeh.Parameters["unit"].SetValue(new Vector2(1f / Width, 1f / Height));
        }

        private void UpdateCamera() {
            if (MouseCondition.Scrolled()) {
                SetExpTween(MathHelper.Clamp(_targetExp - MouseCondition.ScrollDelta * _expDistance, _maxExp, _minExp));
            }

            _camera.Z = _camera.ScaleToZ(ExpToScale(_exp.Value), 0f);
            _mouseWorld = _camera.ScreenToWorld(InputHelper.NewMouse.X, InputHelper.NewMouse.Y);

            if (_dragCamera.Pressed()) {
                _dragAnchor = _mouseWorld;
            }
            if (_dragCamera.Held()) {
                SetXYTween(_xy.Value + _dragAnchor - _mouseWorld, 0);
                _mouseWorld = _dragAnchor;
            }

            _camera.XY = _xy.Value;
        }

        private float ScaleToExp(float scale) {
            return -MathF.Log(scale);
        }
        private float ExpToScale(float exp) {
            return MathF.Exp(-exp);
        }
        private void SetXYTween(float targetX, float targetY, long duration = 1200) {
            SetXYTween(new Vector2(targetX, targetY), duration);
        }
        private void SetXYTween(Vector2 target, long duration = 1200) {
            _xy.A = _xy.Value;
            _xy.B = target;
            _xy.StartTime = TweenHelper.TotalMS;
            _xy.Duration = duration;
        }
        private void SetExpTween(float target, long duration = 1200) {
            _targetExp = target;
            _exp.A = _exp.Value;
            _exp.B = _targetExp;
            _exp.StartTime = TweenHelper.TotalMS;
            _exp.Duration = duration;
        }
        private void SetZTween(float target, long duration = 1200) {
            _targetExp = ScaleToExp(_camera.ZToScale(target, 0f));
            _exp.A = _exp.Value;
            _exp.B = _targetExp;
            _exp.StartTime = TweenHelper.TotalMS;
            _exp.Duration = duration;
        }

        GraphicsDeviceManager _graphics;
        SpriteBatch _s;
        ShapeBatch _sb;

        Camera _camera;

        ICondition _quit =
            new AnyCondition(
                new KeyboardCondition(Keys.Escape),
                new GamePadCondition(GamePadButton.Back, 0)
            );
        ICondition _toggleFullscreen =
            new AllCondition(
                new KeyboardCondition(Keys.LeftAlt),
                new KeyboardCondition(Keys.Enter)
            );
        ICondition _toggleBorderless = new KeyboardCondition(Keys.F11);
        ICondition _dragCamera =
            new AnyCondition(
                new MouseCondition(MouseButton.RightButton),
                new MouseCondition(MouseButton.MiddleButton)
            );

        public static int Width;
        public static int Height;

        RenderTarget2D _target1;
        RenderTarget2D _target2;

        Vector2Tween _xy = new Vector2Tween(Vector2.Zero, Vector2.Zero, 0, Easing.QuintOut);
        FloatTween _exp = new FloatTween(0f, 0f, 0, Easing.QuintOut);

        Vector2 _mouseWorld;
        Vector2 _dragAnchor = Vector2.Zero;

        float _targetExp = 0f;
        float _expDistance = 0.002f;
        float _maxExp = -4f;
        float _minExp = 1f;
    }
}
