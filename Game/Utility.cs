using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GameProject {
    public static class Utility {
        public static void ToggleFullscreen() {
            bool oldIsFullscreen = Assets.Settings.IsFullscreen;

            if (Assets.Settings.IsBorderless) {
                Assets.Settings.IsBorderless = false;
            } else {
                Assets.Settings.IsFullscreen = !Assets.Settings.IsFullscreen;
            }

            ApplyFullscreenChange(oldIsFullscreen);
        }
        public static void ToggleBorderless() {
            bool oldIsFullscreen = Assets.Settings.IsFullscreen;

            Assets.Settings.IsBorderless = !Assets.Settings.IsBorderless;
            Assets.Settings.IsFullscreen = Assets.Settings.IsBorderless;

            ApplyFullscreenChange(oldIsFullscreen);
        }

        public static string GetPath(string name) => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name);
        public static T LoadJson<T>(string name, JsonTypeInfo<T> typeInfo) where T : new() {
            T? json = default;
            string jsonPath = GetPath(name);

            if (File.Exists(jsonPath)) {
                json = JsonSerializer.Deserialize(File.ReadAllText(jsonPath), typeInfo);
            }

            json ??= new T();

            return json;
        }
        public static void SaveJson<T>(string name, T json, JsonTypeInfo<T> typeInfo) {
            string jsonPath = GetPath(name);
            string jsonString = JsonSerializer.Serialize(json, typeInfo);
            File.WriteAllText(jsonPath, jsonString);
        }
        public static T EnsureJson<T>(string name, JsonTypeInfo<T> typeInfo) where T : new() {
            T? json = default;
            string jsonPath = GetPath(name);

            if (File.Exists(jsonPath)) {
                json = JsonSerializer.Deserialize(File.ReadAllText(jsonPath), typeInfo);
            }
            if (json == null) {
                json = new T();
                string jsonString = JsonSerializer.Serialize(json, typeInfo);
                File.WriteAllText(jsonPath, jsonString);
            }

            return json;
        }

        public static void ApplyFullscreenChange(bool oldIsFullscreen) {
            if (Assets.Settings.IsFullscreen) {
                if (oldIsFullscreen) {
                    ApplyHardwareMode();
                } else {
                    SetFullscreen();
                }
            } else {
                UnsetFullscreen();
            }
        }
        public static void ApplyHardwareMode() {
            G.Graphics.HardwareModeSwitch = !Assets.Settings.IsBorderless;
            G.Graphics.ApplyChanges();
        }
        public static void SetFullscreen() {
            SaveWindow();

            G.Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            G.Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            G.Graphics.HardwareModeSwitch = !Assets.Settings.IsBorderless;

            G.Graphics.IsFullScreen = true;
            G.Graphics.ApplyChanges();
        }
        public static void UnsetFullscreen() {
            G.Graphics.IsFullScreen = false;
            RestoreWindow();
        }
        public static void SaveWindow() {
            Assets.Settings.X = G.Window.ClientBounds.X;
            Assets.Settings.Y = G.Window.ClientBounds.Y;
            Assets.Settings.Width = G.Window.ClientBounds.Width;
            Assets.Settings.Height = G.Window.ClientBounds.Height;
        }
        public static void RestoreWindow() {
            G.Window.Position = new Point(Assets.Settings.X, Assets.Settings.Y);
            G.Graphics.PreferredBackBufferWidth = Assets.Settings.Width;
            G.Graphics.PreferredBackBufferHeight = Assets.Settings.Height;
            G.Graphics.ApplyChanges();
        }
    }
}
