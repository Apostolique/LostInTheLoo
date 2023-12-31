﻿using System;
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
        }

        protected override void LoadContent() {
            InputHelper.Setup(this);

            Assets.LoadAssets(Content, GraphicsDevice);

            Window.ClientSizeChanged += ClientSizeChanged;
            ClientSizeChanged(null, null);

            G.S = new SpriteBatch(GraphicsDevice);
            G.B = new Batch(GraphicsDevice);
            G.Camera = new Camera(new DensityViewport(GraphicsDevice, Window, 2000f, 2000f));
            SetExpTween(-1.2f, 0);

            Random rand = new();
            int rand_theme = rand.Next(0, 3);
            Assets.AudioTheme theme = (Assets.AudioTheme)typeof(Assets.AudioTheme).GetEnumValues().GetValue(rand_theme);

            _low = Assets.Themes[theme].music[Assets.MusicIntensity.Low].GetMusic.CreateInstance();
            _medium = Assets.Themes[theme].music[Assets.MusicIntensity.Medium].GetMusic.CreateInstance();
            _mediumHigh = Assets.Themes[theme].music[Assets.MusicIntensity.MediumHigh].GetMusic.CreateInstance();
            _high = Assets.Themes[theme].music[Assets.MusicIntensity.High].GetMusic.CreateInstance();
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

            WorldGenerator.Generate();
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
            Composite?.Dispose();
            Real?.Dispose();
            Imaginary?.Dispose();
            Temp?.Dispose();
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
            WorldTime.Update(G.WorldTime, gameTime);
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

            if (_p.Pressed()) { G.WorldTime.IsPaused = !G.WorldTime.IsPaused; }

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
                var entities = G.EntitiesByLocation.ToArray();
                foreach (var entity in entities)
                {
                    entity.UpdateLogic.Update(entity, gameTime);
                }
            }

            InputHelper.UpdateCleanup();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            _fps.Draw(gameTime);
            G.R.Clear(Composite);
            G.R.Clear(Target1);

            float time = (float)gameTime.TotalGameTime.TotalMilliseconds;
            Assets.Micro.Parameters["Time"].SetValue(time * 0.00005f);
            Assets.Micro.Parameters["SinTime"].SetValue(MathF.Sin(time * 0.001f));

            float intervalGroup = 0.05f;
            float focalPoint = 0.01f;
            float maxFocus = 0.5f;
            float maxOpacity = 0.5f;
            foreach (var group in G.EntitiesByLocation
                .Query(G.Camera.GetViewRect())
                .Where(e => G.Camera.IsZVisible(e.Z, 0.01f))
                .GroupBy(e => MathF.Round(e.Z / intervalGroup) * intervalGroup)) // changed from floor to round to limit the "teleport" from worst case almost 1.0f to 0.5f if Z distance
            {
                G.B.Begin(view: G.Camera.GetView(group.Key));
                foreach (var entity in group.OrderBy(e => e)) {
                    entity.RenderLogic.Render(entity);
                }
                G.B.End();
                float blur = MathF.Abs(group.Key - focalPoint) / maxFocus * G.Camera.WorldToScreenScale();
                float opacity = MathF.Max(1f - MathF.Abs(group.Key - focalPoint) / maxOpacity, 0f);
                G.R.ApplyBokeh(Target1, Real, Imaginary, Temp, Composite, group.Key, blur, opacity);
            }

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(TWColor.Black);

            G.S.Begin(transformMatrix: G.Camera.GetView(-3f));
            G.S.Draw(Assets.Background4, new Rectangle(-20000, -20000, Assets.Background4.Width * 30, Assets.Background4.Height * 30), TWColor.White);
            G.S.End();

            G.R.Draw(Composite);

            // G.B.Begin();
            // G.B.Draw(Batch.MicroShapes.Ovoid, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(0, 0)));
            // G.B.Draw(Batch.MicroShapes.Ovoid2, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(256, 0)));
            // G.B.Draw(Batch.MicroShapes.Cylindrical, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(512, 0)));
            // G.B.Draw(Batch.MicroShapes.Cylindrical2, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(768, 0)));
            // G.B.Draw(Batch.MicroShapes.Cylindrical3, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1024, 0)));
            // G.B.Draw(Batch.MicroShapes.Cylindrical4, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1280, 0)));
            // G.B.Draw(Batch.MicroShapes.Cylindrical5, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(0, 256)));
            // G.B.Draw(Batch.MicroShapes.Triangle, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(256, 256)));
            // G.B.Draw(Batch.MicroShapes.Triangle2, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(512, 256)));
            // G.B.Draw(Batch.MicroShapes.Triangle3, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(768, 256)));
            // G.B.Draw(Batch.MicroShapes.Triangle4, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1024, 256)));
            // G.B.Draw(Batch.MicroShapes.Triangle5, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1280, 256)));
            // G.B.Draw(Batch.MicroShapes.Triangle6, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(0, 512)));
            // G.B.Draw(Batch.MicroShapes.Triangle7, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(256, 512)));
            // G.B.Draw(Batch.MicroShapes.Triangle8, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(512, 512)));
            // G.B.Draw(Batch.MicroShapes.Triangle9, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(768, 512)));
            // G.B.Draw(Batch.MicroShapes.Triangle10, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1024, 512)));
            // G.B.Draw(Batch.MicroShapes.Square, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1280, 512)));
            // G.B.Draw(Batch.MicroShapes.Square2, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(0, 768)));
            // G.B.Draw(Batch.MicroShapes.Square3, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(256, 768)));
            // G.B.Draw(Batch.MicroShapes.Square4, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(512, 768)));
            // G.B.Draw(Batch.MicroShapes.Square5, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(768, 768)));
            // G.B.Draw(Batch.MicroShapes.Square6, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1024, 768)));
            // G.B.Draw(Batch.MicroShapes.Skewer, Batch.MicroRamps.Ramp02, 0.3f, 0.8f, new Vector2(0f, 1f), Matrix32.CreateScale(256, 256) * Matrix32.CreateTranslation(new Vector2(1280, 768)));
            // G.B.End();

            var font = Assets.FontSystem.GetFont(24);
            G.S.Begin();
            G.S.DrawString(font, $"fps: {_fps.FramesPerSecond} - Dropped Frames: {_fps.DroppedFrames} - Draw ms: {_fps.TimePerFrame} - Update ms: {_fps.TimePerUpdate} - Entities: {G.EntitiesByLocation.Count} - TotalGameTime: {gameTime.TotalGameTime} - TotalWorldTime: {G.WorldTime.TotalGameTime}", new Vector2(10, 10), TWColor.White);
            G.S.End();

            base.Draw(gameTime);
        }

        private void CreateTargets() {
            Target1 = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Composite = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            Real = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            Imaginary = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Vector4, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            Temp = new RenderTarget2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            Assets.BokehVertical.Parameters["unit"]?.SetValue(new Vector2(1f / Width, 1f / Height));
            Assets.BokehHorizontal.Parameters["unit"]?.SetValue(new Vector2(1f / Width, 1f / Height));
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
                new MouseCondition(MouseButton.LeftButton),
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

        ICondition _p = new KeyboardCondition(Keys.P);

        float _maxVolume = 0.3f;
        FloatTween _lowVolume = new FloatTween(0f, 0.3f, 5000, Easing.Linear);
        FloatTween _mediumVolume = new FloatTween(0f, 0f, 5000, Easing.Linear);
        FloatTween _mediumHighVolume = new FloatTween(0f, 0f, 5000, Easing.Linear);
        FloatTween _highVolume = new FloatTween(0f, 0f, 5000, Easing.Linear);
        FloatTween _currentTrack;

        public static int Width;
        public static int Height;

        public static RenderTarget2D Target1;
        public static RenderTarget2D Composite;
        public static RenderTarget2D Real;
        public static RenderTarget2D Imaginary;
        public static RenderTarget2D Temp;

        Vector2Tween _xy = new Vector2Tween(Vector2.Zero, Vector2.Zero, 0, Easing.QuintOut);
        FloatTween _exp = new FloatTween(0f, 0f, 0, Easing.QuintOut);


        Vector2 _mouseWorld;
        Vector2 _dragAnchor = Vector2.Zero;

        float _targetExp = 0.24f;
        float _expDistance = 0.002f;
        float _maxExp = -4f;
        float _minExp = 1f;
    }
}
