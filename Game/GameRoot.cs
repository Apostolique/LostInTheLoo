using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Apos.Input;
using Apos.Camera;
using Apos.Tweens;
using FontStashSharp;

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
            G.Setup(this, _graphics);

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

            G.S = new SpriteBatch(GraphicsDevice);
            G.SB = new ShapeBatch(GraphicsDevice);
            G.Camera = new Camera(new DensityViewport(GraphicsDevice, Window, 2000f, 2000f));

            a1 = new Moves.Line(new Vector2(0f, 0f), new Vector2(1f, 1f));
        }

        protected void ClientSizeChanged(object? sender, EventArgs? e) {
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
            _target3?.Dispose();
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
            _fps.Update(gameTime);

            if (_quit.Pressed())
                Exit();

            if (_toggleFullscreen.Pressed()) {
                Utility.ToggleFullscreen();
            }
            if (_toggleBorderless.Pressed()) {
                Utility.ToggleBorderless();
            }

            UpdateCamera();

            a1.Update(gameTime);

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            _fps.Draw(gameTime);
            G.R.Clear(_target2);
            G.R.Clear(_target1);

            G.SB.Begin(view: G.Camera.GetView(-0.2f));
            G.SB.DrawCircle(new Vector2(100f, 100f), 20f, TWColor.Red500, TWColor.White, 2f);
            G.SB.End();
            G.R.ApplyBokeh(_target1, _target2, 10f, -0.2f);

            G.SB.Begin(view: G.Camera.GetView(-0.1f));
            G.SB.DrawCircle(new Vector2(0f, 0f), 20f, TWColor.Blue200, TWColor.Black, 1f);
            G.SB.End();
            G.R.ApplyBokeh(_target1, _target2, 2f, -0.1f);

            G.SB.Begin(view: G.Camera.GetView(0f));
            G.SB.DrawEllipse(a1.XY, 50f, 20f, TWColor.Pink300, TWColor.Gray800, 1f);
            G.SB.End();
            G.R.DrawTo(_target1, _target2);

            G.R.DrawInfinite(Assets.Noise1, _target3, 0f, 1f, Vector2.Zero);

            G.GraphicsDevice.SetRenderTarget(_target1);
            G.GraphicsDevice.Clear(TWColor.Black);
            G.SB.Begin(view: G.Camera.GetView(0f));
            G.SB.FillCircle(new Vector2(-100f, -100f), 20f, TWColor.White);
            G.SB.End();
            G.R.ApplyMask(_target3, _target1, _target2);

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(TWColor.Black);

            G.S.Begin(transformMatrix: G.Camera.GetView(-3f));
            G.S.Draw(Assets.Background, new Rectangle(-15000, -10000, Assets.Background.Width * 10, Assets.Background.Height * 10), TWColor.White);
            G.S.End();

            G.R.Draw(_target2);

            var font = Assets.FontSystem.GetFont(24);
            G.S.Begin();
            G.S.DrawString(font, $"fps: {_fps.FramesPerSecond} - Dropped Frames: {_fps.DroppedFrames} - Draw ms: {_fps.TimePerFrame} - Update ms: {_fps.TimePerUpdate}", new Vector2(10, 10), TWColor.White);
            G.S.End();

            base.Draw(gameTime);
        }

        private void CreateTargets() {
            _target1 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _target2 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            _target3 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Assets.Bokeh.Parameters["unit"].SetValue(new Vector2(1f / Width, 1f / Height));
        }

        private void UpdateCamera() {
            if (MouseCondition.Scrolled()) {
                SetExpTween(MathHelper.Clamp(_targetExp - MouseCondition.ScrollDelta * _expDistance, _maxExp, _minExp));
            }

            G.Camera.Z = G.Camera.ScaleToZ(ExpToScale(_exp.Value), 0f);
            _mouseWorld = G.Camera.ScreenToWorld(InputHelper.NewMouse.X, InputHelper.NewMouse.Y);

            if (_dragCamera.Pressed()) {
                _dragAnchor = _mouseWorld;
            }
            if (_dragCamera.Held()) {
                SetXYTween(_xy.Value + _dragAnchor - _mouseWorld, 0);
                _mouseWorld = _dragAnchor;
            }

            G.Camera.XY = _xy.Value;
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
            _targetExp = ScaleToExp(G.Camera.ZToScale(target, 0f));
            _exp.A = _exp.Value;
            _exp.B = _targetExp;
            _exp.StartTime = TweenHelper.TotalMS;
            _exp.Duration = duration;
        }

        GraphicsDeviceManager _graphics;

        FPSCounter _fps = new FPSCounter();

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
        RenderTarget2D _target3;

        Vector2Tween _xy = new Vector2Tween(Vector2.Zero, Vector2.Zero, 0, Easing.QuintOut);
        FloatTween _exp = new FloatTween(0f, 0f, 0, Easing.QuintOut);

        Vector2 _mouseWorld;
        Vector2 _dragAnchor = Vector2.Zero;

        float _targetExp = 0f;
        float _expDistance = 0.002f;
        float _maxExp = -4f;
        float _minExp = 1f;

        Moves.Line a1;
    }
}
