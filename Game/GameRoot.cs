using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Apos.Input;
using Apos.Camera;
using Apos.Tweens;
using FontStashSharp;
using MonoGame.Extended;
using System.Linq;
using Microsoft.Xna.Framework.Audio;

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

            WorldGenerator.Generate();
        }

        protected override void LoadContent() {
            InputHelper.Setup(this);

            Assets.LoadAssets(Content, GraphicsDevice);

            Window.ClientSizeChanged += ClientSizeChanged;
            ClientSizeChanged(null, null);

            G.S = new SpriteBatch(GraphicsDevice);
            G.SB = new ShapeBatch(GraphicsDevice);
            G.Camera = new Camera(new DensityViewport(GraphicsDevice, Window, 2000f, 2000f));

            _low = Assets.Low.CreateInstance();
            _medium = Assets.Medium.CreateInstance();
            _mediumHigh = Assets.MediumHigh.CreateInstance();
            _high = Assets.High.CreateInstance();
            _low.IsLooped = true;
            _medium.IsLooped = true;
            _mediumHigh.IsLooped = true;
            _high.IsLooped = true;
            _low.Volume = 0f;
            _medium.Volume = 0f;
            _mediumHigh.Volume = 0f;
            _high.Volume = 0f;
            _low.Play();
            _medium.Play();
            _mediumHigh.Play();
            _high.Play();
            _currentTrack = _lowVolume;
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

            Target1?.Dispose();
            Target2?.Dispose();
            Target3?.Dispose();
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

            if (_0.Pressed()) { SetVolume(0.0f); }
            if (_1.Pressed()) { SetVolume(0.2f); }
            if (_2.Pressed()) { SetVolume(0.3f); }
            if (_3.Pressed()) { SetVolume(0.4f); }
            if (_4.Pressed()) { SetVolume(0.5f); }
            if (_5.Pressed()) { SetVolume(0.6f); }
            if (_6.Pressed()) { SetVolume(0.7f); }
            if (_7.Pressed()) { SetVolume(0.8f); }
            if (_8.Pressed()) { SetVolume(0.9f); }
            if (_9.Pressed()) { SetVolume(1.0f); }

            if (_lowTrigger.Pressed()) {
                _currentTrack = _lowVolume;
                FadeTrack(_lowVolume, _maxVolume);
                FadeTrack(_mediumVolume, 0f);
                FadeTrack(_mediumHighVolume, 0f);
                FadeTrack(_highVolume, 0f);
            }
            if (_mediumTrigger.Pressed()) {
                _currentTrack = _mediumVolume;
                FadeTrack(_lowVolume, 0f);
                FadeTrack(_mediumVolume, _maxVolume);
                FadeTrack(_mediumHighVolume, 0f);
                FadeTrack(_highVolume, 0f);
            }
            if (_mediumHighTrigger.Pressed()) {
                _currentTrack = _mediumHighVolume;
                FadeTrack(_lowVolume, 0f);
                FadeTrack(_mediumVolume, 0f);
                FadeTrack(_mediumHighVolume, _maxVolume);
                FadeTrack(_highVolume, 0f);
            }
            if (_highTrigger.Pressed()) {
                _currentTrack = _highVolume;
                FadeTrack(_lowVolume, 0f);
                FadeTrack(_mediumVolume, 0f);
                FadeTrack(_mediumHighVolume, 0f);
                FadeTrack(_highVolume, _maxVolume);
            }

            _low.Volume = _lowVolume.Value;
            _medium.Volume = _mediumVolume.Value;
            _mediumHigh.Volume = _mediumHighVolume.Value;
            _high.Volume = _highVolume.Value;

            UpdateCamera();

            if(gameTime.ElapsedGameTime.TotalSeconds > 0.0d)
            {
                _entitiesInView = G.EntitiesByLocation.Query(G.Camera.GetViewRect()).OrderBy(e => e.Z).Where(e => G.Camera.IsZVisible(e.Z, 0.01f)).ToArray();
                foreach (var entity in _entitiesInView)
                {
                    entity.UpdateLogic.Update(entity, gameTime);
                }
            }

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            _fps.Draw(gameTime);
            G.R.Clear(Target2);
            G.R.Clear(Target1);

            float intervalGroup = 0.1f;
            foreach (var group in _entitiesInView.GroupBy(e => MathF.Floor(e.Z / intervalGroup + 0.5f) * intervalGroup))
            {
                G.SB.Begin(view: G.Camera.GetView(group.Key));
                foreach (var entity in group) {
                    entity.RenderLogic.Render(entity);
                }
                G.SB.End();
                G.R.ApplyBokeh(GameRoot.Target1, GameRoot.Target2, 0f, group.Key);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(TWColor.Black);

            G.S.Begin(transformMatrix: G.Camera.GetView(-3f));
            G.S.Draw(Assets.Background, new Rectangle(-15000, -10000, Assets.Background.Width * 10, Assets.Background.Height * 10), TWColor.White);
            G.S.End();

            G.R.Draw(Target2);

            var font = Assets.FontSystem.GetFont(24);
            G.S.Begin();
            G.S.DrawString(font, $"fps: {_fps.FramesPerSecond} - Dropped Frames: {_fps.DroppedFrames} - Draw ms: {_fps.TimePerFrame} - Update ms: {_fps.TimePerUpdate}", new Vector2(10, 10), TWColor.White);
            G.S.End();

            base.Draw(gameTime);
        }

        private void CreateTargets() {
            Target1 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Target2 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Target3 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

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

        private void SetVolume(float v) {
            _maxVolume = v;

            CapVolume(_lowVolume, _maxVolume);
            CapVolume(_mediumVolume, _maxVolume);
            CapVolume(_mediumHighVolume, _maxVolume);
            CapVolume(_highVolume, _maxVolume);
        }
        private void CapVolume(FloatTween ft, float v) {
            if (_currentTrack == ft) {
                _currentTrack.B = v;
            } else if (ft.A > v) {
                ft.A = v;
            }
        }
        private void FadeTrack(FloatTween ft, float v) {
            ft.A = ft.Value;
            ft.B = v;
            ft.StartTime = TweenHelper.TotalMS;
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

        ICondition _lowTrigger = new KeyboardCondition(Keys.D1);
        ICondition _mediumTrigger = new KeyboardCondition(Keys.D2);
        ICondition _mediumHighTrigger = new KeyboardCondition(Keys.D3);
        ICondition _highTrigger = new KeyboardCondition(Keys.D4);
        SoundEffectInstance _low;
        SoundEffectInstance _medium;
        SoundEffectInstance _mediumHigh;
        SoundEffectInstance _high;

        ICondition _0 = new KeyboardCondition(Keys.NumPad0);
        ICondition _1 = new KeyboardCondition(Keys.NumPad1);
        ICondition _2 = new KeyboardCondition(Keys.NumPad2);
        ICondition _3 = new KeyboardCondition(Keys.NumPad3);
        ICondition _4 = new KeyboardCondition(Keys.NumPad4);
        ICondition _5 = new KeyboardCondition(Keys.NumPad5);
        ICondition _6 = new KeyboardCondition(Keys.NumPad6);
        ICondition _7 = new KeyboardCondition(Keys.NumPad7);
        ICondition _8 = new KeyboardCondition(Keys.NumPad8);
        ICondition _9 = new KeyboardCondition(Keys.NumPad9);

        float _maxVolume = 0.3f;
        FloatTween _lowVolume = new FloatTween(0f, 0.3f, 20000, Easing.Linear);
        FloatTween _mediumVolume = new FloatTween(0f, 0f, 20000, Easing.Linear);
        FloatTween _mediumHighVolume = new FloatTween(0f, 0f, 20000, Easing.Linear);
        FloatTween _highVolume = new FloatTween(0f, 0f, 20000, Easing.Linear);
        FloatTween _currentTrack;

        public static int Width;
        public static int Height;

        public static RenderTarget2D Target1;
        public static RenderTarget2D Target2;
        public static RenderTarget2D Target3;

        Vector2Tween _xy = new Vector2Tween(Vector2.Zero, Vector2.Zero, 0, Easing.QuintOut);
        FloatTween _exp = new FloatTween(0f, 0f, 0, Easing.QuintOut);

        Vector2 _mouseWorld;
        Vector2 _dragAnchor = Vector2.Zero;

        float _targetExp = 0f;
        float _expDistance = 0.002f;
        float _maxExp = -4f;
        float _minExp = 1f;

        Entity[] _entitiesInView = Array.Empty<Entity>();
    }
}
