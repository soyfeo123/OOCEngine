# OutOfComfort (OOC) Game Engine
OOC is a game engine made entirely in C# and a SDL2 wrapper, inspired by the Source Engine by Valve.

# Make with ease
You don't have to clone this repo to make games for it! If you're coming from Source, it's a clear path (no guarantees :shrug:). OOC has a very similar file structure to Source's.

# Current features
- Full Lua support
- Rendering
- UI
- Modding support
- Full asset manager
  
  More coming soon.

# Building OOC
1. Right click the OOCEngine.v(current version) project, and click Properties.
2. Navigate over to "Build Events", and on Post-build event command line, replace whatever's in there to this code:
   ```
   cd C:\(bin/Debug file path)
   move /Y *.dll bin
   move /Y *.xml bin
   ```
3. Replace (bin/Debug file path) with... you guessed it, the bin/Debug file path.
4. Download the devel ZIP files for [SDL](https://github.com/libsdl-org/SDL/releases/tag/release-2.30.0), [SDL_image](https://github.com/libsdl-org/SDL_image/releases/tag/release-2.8.2) and [SDL_ttf](https://github.com/libsdl-org/SDL_ttf/releases/tag/release-2.22.0).
5. Get the DLL from extracting the ZIP file, and find  then place it into the bin folder (inside bin/Debug).
6. Repeat step 5 through all the downloaded ZIP files.
7. Afterwards, build by going into Build > Build Solution, and press "Start" to start your OOC engine!
