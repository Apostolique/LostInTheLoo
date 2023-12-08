using FontStashSharp;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameProject 
{
  public static class Assets 
  {
    public static void LoadAssets(ContentManager content, GraphicsDevice graphicsDevice) {
        LoadFonts(content);
        LoadTextures(content);
        LoadShaders(content);
        LoadSounds(content);
    }

    public static void LoadJson() 
    {
        Settings = Utility.EnsureJson("Settings.json", SettingsContext.Default.Settings);
    }
    public static void LoadFonts(ContentManager content) 
    {
        FontSystem = new FontSystem();
        FontSystem.AddFont(TitleContainer.OpenStream($"{content.RootDirectory}/source-code-pro-medium.ttf"));
    }
    private static void LoadTextures(ContentManager content) 
    {
        Background = content.Load<Texture2D>("background");
        Background3 = content.Load<Texture2D>("background3");
        Noise1 = content.Load<Texture2D>("noise1");
        Noise2 = content.Load<Texture2D>("noise2");
        Mask1 = content.Load<Texture2D>("mask1");
        MicroRamps = content.Load<Texture2D>("textures/ramps/ramp-sheets");
        MicroShapes = content.Load<Texture2D>("textures/shapes/shape-sprites");
        Core01 = content.Load<Texture2D>("textures/cores/core_01");
        Core02 = content.Load<Texture2D>("textures/cores/core_02");
        Bean = content.Load<Texture2D>("textures/shapes/shape_bean2");
        Bell = content.Load<Texture2D>("textures/shapes/shape_bell");
        Drill = content.Load<Texture2D>("textures/shapes/shape_drill");
        Skewer = content.Load<Texture2D>("textures/shapes/shape_skewer");
    }
    private static void LoadShaders(ContentManager content) 
    {
        Shapes = content.Load<Effect>("apos-shapes");
        BokehVertical = content.Load<Effect>("bokeh-vertical");
        BokehHorizontal = content.Load<Effect>("bokeh-horizontal");
        Infinite = content.Load<Effect>("infinite");
        Mask = content.Load<Effect>("mask");
        Micro = content.Load<Effect>("microorganism");
    }
    private static void LoadSounds(ContentManager content) 
    {
      foreach (AudioTheme t in Enum.GetValues(typeof(AudioTheme)))
        Themes.Add(t,new ThemeInfo(t, content));
    }

    public enum AudioTheme
    {
      T1,
      T2,
      T3,
    }

    public enum MusicIntensity
    {
      Low,
      Medium,
      MediumHigh,
      High
    }

    public enum SFXType 
    {
      S1,
      S2,
      S3,
      S4,
      Death,
    }

    public class ThemeInfo
    {
      AudioTheme theme;
      public Dictionary<SFXType, SoundInfo> sounds = [];
      public Dictionary<MusicIntensity, MusicInfo> music = [];

      public ThemeInfo(AudioTheme theme, ContentManager content)
      {
        this.theme = theme;
        foreach (SFXType sft in Enum.GetValues(typeof(SFXType)))
          sounds.Add(sft,new SoundInfo(content, sft, theme));
        foreach (MusicIntensity mi in Enum.GetValues(typeof(MusicIntensity)))
          music.Add(mi,new MusicInfo(content, mi, theme));
      }
      public AudioTheme GetTheme => theme;
    }

    public class SoundInfo
    {
      SoundEffect sfx;
      SFXType type;

      public SoundInfo(ContentManager content, SFXType type, AudioTheme t) 
      {
        this.type = type;
        string type_text = type.ToString();
        string theme_text = t.ToString();
        string location = "music/" + theme_text + "/LITL " + theme_text + " " + type_text;
        sfx = content.Load<SoundEffect>(location);
      }

      public SoundEffect GetSound => sfx;
    }

    public class MusicInfo 
    {
      SoundEffect music;
      MusicIntensity intensity;

      public MusicInfo(ContentManager content, MusicIntensity intensity, AudioTheme t)
      {
        this.intensity = intensity; 
        string intensity_text = intensity.ToString();
        if (intensity_text == "MediumHigh") intensity_text = "Medium High"; //Change the filenames, not the enum
        string theme_text = t.ToString();
        string location = "music/" + theme_text + "/LITL " + theme_text + " " + intensity_text;
        music = content.Load<SoundEffect>(location);
      }
      public SoundEffect GetMusic => music;
    }

    public static Settings Settings;
    public static FontSystem FontSystem;

    public static Texture2D Background;
    public static Texture2D Background3;
    public static Texture2D Noise1;
    public static Texture2D Noise2;
    public static Texture2D Mask1;
    public static Texture2D MicroRamps;
    public static Texture2D MicroShapes;
    public static Texture2D Core01;
    public static Texture2D Core02;
    public static Texture2D Bean;
    public static Texture2D Bell;
    public static Texture2D Drill;
    public static Texture2D Skewer;

    public static Effect BokehVertical;
    public static Effect BokehHorizontal;
    public static Effect Shapes;
    public static Effect Infinite;
    public static Effect Mask;
    public static Effect Micro;

    public static Dictionary<AudioTheme, ThemeInfo> Themes = [];

  }
}
