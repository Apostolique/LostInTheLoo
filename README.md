# LostInTheLoo
My entry for the MonoGameJam5.

## Restore

```
dotnet restore Platforms/DesktopGL
dotnet restore Platforms/WindowsDX
```

## Run

```
dotnet run --project Platforms/DesktopGL
dotnet run --project Platforms/WindowsDX
```

## MonoGame Content Pipeline Editor

To run the MonoGame content pipeline editor, you can use the following command:

```
dotnet mgcb-editor Content/Content.mgcb
```

## Debug

In vscode, you can debug by pressing F5.

## Publish

```
dotnet publish Platforms/DesktopGL -c Release -r win-x64 --output artifacts/windows
dotnet publish Platforms/DesktopGL -c Release -r osx-x64 --output artifacts/osx
dotnet publish Platforms/DesktopGL -c Release -r linux-x64 --output artifacts/linux
```

```
dotnet publish Platforms/WindowsDX -c Release -r win-x64 --output artifacts/windowsdx
```

## Project structure so far

* Assets.cs is mostly for loading assets without cluttering the code.
* Utility.cs is for putting code that we don't really care to organize. Useful for not using brain.
* Settings.cs saves and loads values from the last game session. The file is placed next to the game exe.
* ShapeBatch and ShapeVertex is the [Apos.Shapes](https://github.com/Apostolique/Apos.Shapes) library. Brought manually to the project in case we want to modify it.
* TWColor are useful colors that can be used quickly without thinking. Preview here: <https://tailwindcss.com/docs/customizing-colors#default-color-palette>.

Some libraries that are already included:

* [Apos.Camera](https://github.com/Apostolique/Apos.Camera)
* [Apos.Input](https://github.com/Apostolique/Apos.Input)
* [Apos.Spatial](https://github.com/Apostolique/Apos.Spatial)
* [Apos.Tweens](https://github.com/Apostolique/Apos.Tweens)
