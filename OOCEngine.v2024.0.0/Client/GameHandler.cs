using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OOCEngine;
using System.IO;
using OOCEngine.LowLevel;
using SDL2;
using static SDL2.SDL;
using static SDL2.SDL_image;
using static SDL2.SDL_mixer;
using static SDL2.SDL_ttf;
using System.Windows.Forms;
using System.Runtime.Remoting;
using MoonSharp.Interpreter;

namespace OOCEngine
{
    public class GameInfo
    {
        public int Priority;
        public string Game;
        public string Title;
        public string Path;
        public static GameInfo GetWithHighestPriority()
        {
            Dictionary<GameInfo, int> games = new Dictionary<GameInfo, int>();
            foreach(GameInfo game in Program.gameInfoList) { games.Add(game, game.Priority); }
            return games.OrderByDescending(x => x.Value).FirstOrDefault().Key;
        }
    }

    public static class OOCUtil
    {
        public static IntPtr LoadTexture(string filename)
        {
            Dictionary<string, int> gameTextures = new Dictionary<string, int>();
            foreach(GameInfo game in Program.gameInfoList)
            {
                string newFileName = Path.Combine(game.Path, "texture", filename);
                if (File.Exists(newFileName))
                {
                    gameTextures.Add(newFileName, game.Priority);
                }
            }
            var filePath = gameTextures.OrderByDescending(x => x.Value).FirstOrDefault();

            

            return IMG_LoadTexture(Program.renderer, filePath.Key);
        }

        public static IntPtr LoadMusic(string filename)
        {
            Dictionary<string, int> gameTextures = new Dictionary<string, int>();
            foreach (GameInfo game in Program.gameInfoList)
            {
                string newFileName = Path.Combine(game.Path, "sound", filename);
                if (File.Exists(newFileName))
                {
                    gameTextures.Add(newFileName, game.Priority);
                }
            }
            var filePath = gameTextures.OrderByDescending(x => x.Value).FirstOrDefault();



            return Mix_LoadMUS(filePath.Key);
        }

        public static string LoadResource(string section, string filename)
        {
            Dictionary<string, int> gameTextures = new Dictionary<string, int>();
            foreach (GameInfo game in Program.gameInfoList)
            {
                string newFileName = Path.Combine(game.Path, section, filename);
                if (File.Exists(newFileName))
                {
                    gameTextures.Add(newFileName, game.Priority);
                }
            }
            var filePath = gameTextures.OrderByDescending(x => x.Value).FirstOrDefault();

            return File.ReadAllText(filePath.Key);
        }

        public static string GetResourcePath(string section, string filename)
        {
            Dictionary<string, int> gameTextures = new Dictionary<string, int>();
            foreach (GameInfo game in Program.gameInfoList)
            {
                string newFileName = Path.Combine(game.Path, section, filename);
                if (File.Exists(newFileName))
                {
                    gameTextures.Add(newFileName, game.Priority);
                }
            }
            var filePath = gameTextures.OrderByDescending(x => x.Value).FirstOrDefault();

            return filePath.Key;
        }
    }

    public class GameMapManager : GameComponent
    {
        public struct RenderColor
        {
            public byte r;
            public byte g;
            public byte b;
            public byte a;
        }

        private string mapPath;
        private IntPtr _renderer;

        public GameMapManager(string mapFile, IntPtr renderer)
        {
            mapPath = mapFile;
            _renderer = renderer;
        }

        public override void Start()
        {
            
        }

        public override void Draw()
        {
            SDL_SetRenderDrawColor(_renderer, 255, 255, 255, 255);
        }
    }

    public class OGUIHandler : GameComponent
    {
        public static List<OGUIObject> objects = new List<OGUIObject>();
        public override void Draw()
        {
            foreach(OGUIObject obj in objects)
            {
                obj.DrawObject();
            }
        }
        public override void HandleEvent(SDL_Event e)
        {
            foreach(OGUIObject obj in objects)
            {
                obj.Core_HandleEvent(e);
            }
        }
    }
    [MoonSharpUserData]
    public class OGUIObject
    {
        public Vector2px position;
        public Vector2px boundarySize;

        private bool hasAlreadyRegisteredHover = false;
        public virtual void DrawObject()
        {
            int x, y;
            SDL_GetMouseState(out x, out y);
            if(((x > position.x && x < position.x + boundarySize.x) && (y > position.y && y < position.y + boundarySize.y)))
            {
                if (!hasAlreadyRegisteredHover)
                {
                    hasAlreadyRegisteredHover = true;
                    OnHover();
                }
                
            }
            else
            {
                if (hasAlreadyRegisteredHover)
                {
                    hasAlreadyRegisteredHover = false;
                    OnUnhover();
                }
            }
        }
        public virtual void OnHover() { }
        public virtual void OnUnhover() { }
        public virtual void OnClick() { }
        public void Core_HandleEvent(SDL_Event eve)
        {
            if(eve.type == SDL_EventType.SDL_MOUSEBUTTONDOWN && hasAlreadyRegisteredHover)
            {
                OnClick();
            }
        }
    }
    [MoonSharpUserData]
    public class OGUI_Label : OGUIObject
    {
        private IntPtr textTexture;
        private Vector2px location;
        public OGUI_Label(string text, int x, int y)
        {
            IntPtr textSurface = TTF_RenderText_Solid(GameHandler.globalFont, text, new SDL_Color()
            {
                r = Convert.ToByte(255),
                g = Convert.ToByte(255),
                b = Convert.ToByte(255),
                a = Convert.ToByte(255)
            }) ;
            textTexture = SDL_CreateTextureFromSurface(Program.renderer, textSurface);
            location = new Vector2px(x, y);
        }
        public override void DrawObject()
        {
            int w = 0, h = 0;
            uint empty = 0;
            int empty_ = 0;

            SDL_Rect texRect = new SDL_Rect()
            {
                w = w,
                h = h,
                x = location.x,
                y = location.y
            };

            SDL_Rect emptyPtr = new SDL_Rect();
            SDL_QueryTexture(textTexture, out empty, out empty_, out w, out h);
            SDL_RenderCopy(Program.renderer, textTexture, ref emptyPtr, ref texRect);
        }
    }

    [MoonSharpUserData]
    public class OGUILuaWrapper
    {
        public OGUI_Label CreateLabel(string text, int x, int y)
        {
            OGUI_Label labelObj = new OGUI_Label(text, x, y);
            OGUIHandler.objects.Add(labelObj);
            return labelObj;
        }
        public void RemoveObject(OGUIObject obj)
        {
            OGUIHandler.objects.Remove(obj);
        }
    }

    public class GameHandler
    {
        public IntPtr testingTexture;
        public IntPtr testingMusic;
        public static IntPtr globalFont;
        public GameComponent currentGameComponent = new GameComponent();

        public OGUIHandler oguiHandler = new OGUIHandler();

        public Vector2px GetWindowSize()
        {
            return new Vector2px(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
        }

        public void Start()
        {
            //GameMapManager map = new GameMapManager("test.ocm", Program.renderer);
            //ChangeState(map);
            UserData.RegisterAssembly();

            globalFont = TTF_OpenFont(OOCUtil.GetResourcePath("font", "main.ttf"), 70);

            oguiHandler.Start();
            //Debug.Log(OOCUtil.LoadResource("script", "ui/menuDraw.lua"));
            Script menuScript = new Script();
            menuScript.Globals["OGUI"] = new OGUILuaWrapper();
            menuScript.DoString(OOCUtil.LoadResource("script", "ui/menuDraw.lua"));
            
        }

        public void ChangeState(GameComponent newGameComponent)
        {
            currentGameComponent.Cleanup();
            currentGameComponent = newGameComponent;
            currentGameComponent.Start();
        }

        public void Draw()
        {
            currentGameComponent.Draw();
            oguiHandler.Draw();
        }

        public void HandleEvent(SDL_Event ev)
        {
            if(ev.type == SDL_EventType.SDL_KEYDOWN)
            {
                if (ev.key.keysym.sym == SDL_Keycode.SDLK_ESCAPE)
                {
                    SDL_Event events = new SDL_Event()
                    {
                        type = SDL_EventType.SDL_QUIT
                    };
                    SDL_PushEvent(ref events);
                }
            }
            currentGameComponent.HandleEvent(ev);
        }
    }

    public class GameComponent
    {
        public virtual void Draw()
        {

        }
        public virtual void HandleEvent(SDL_Event e)
        {

        }
        public virtual void Start()
        {

        }

        public virtual void Cleanup() { }
    }
}
