using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SDL2;
using System.IO;
using System.Windows.Forms;
using static SDL2.SDL;
using static SDL2.SDL_image;
using Newtonsoft.Json;
using static SDL2.SDL_mixer;
using static SDL2.SDL_ttf;
//using OpenTK;
//using OpenTK.Graphics.OpenGL;

namespace OOCEngine.LowLevel
{
    public static class Program
    {
        static string[] programArguments;
        public static IntPtr window;
        public static IntPtr renderer;
        public static GameHandler gameHandlerInstance;

        public static Vector2px windowPos;
        public static Vector2px windowSize;

        //public static int gProgramID = 0;
        //public static int gVertexPos2DLocation = -1;
        //public static int gVBO;
        //public static int gIBO;

        public static List<GameInfo> gameInfoList = new List<GameInfo>();

        static void Main(string[] args)
        {
            programArguments = args;

            initGameInfos();
            initSDL();
            //initGL();

            gameHandlerInstance.Start();

            SDL_Event e;
            bool quit = false;
            while (quit == false) 
            {

                SDL_RenderClear(renderer);
                gameHandlerInstance.Draw();
                SDL_RenderPresent(renderer);
                while (SDL_PollEvent(out e) != 0) 
                {
                    if (e.type == SDL_EventType.SDL_QUIT) quit = true; else
                    {
                        gameHandlerInstance.HandleEvent(e);
                    }
                }

            }

            

            ExitEvent(null, null);
        }

        public static void RenderCopy(IntPtr texture)
        {
            SDL_RenderCopy(renderer, texture, IntPtr.Zero, IntPtr.Zero);
        }

        public static void initGameInfos()
        {
            foreach(string directory in Directory.GetDirectories(Path.Combine(AppDomain.CurrentDomain.BaseDirectory)))
            {
                if (File.Exists(Path.Combine(directory, "gameinfo.txt")))
                {
                    GameInfo info = JsonConvert.DeserializeObject<GameInfo>(File.ReadAllText(Path.Combine(directory, "gameinfo.txt")));
                    info.Path = directory;
                    gameInfoList.Add(info);
                }
            }
        }

        public static void initSDL()
        {
            

            gameHandlerInstance = new GameHandler();
            windowSize = gameHandlerInstance.GetWindowSize();

            try
            {
                if (SDL_Init(SDL_INIT_VIDEO | SDL_INIT_AUDIO) < 0)
                {
                    Debug.Log(SDL_GetError(), "Init Error", ConsoleColor.Red);
                    MessageBox.Show("Error while initializing SDL.\nGame launch halted.", "OOC Engine Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    ExitEvent(null, null);
                }
                else
                {
                    Debug.Log("Initialized SDL_Video: no errors so far", "LL: SDL Init");

                    /*SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_MULTISAMPLEBUFFERS, 1);
                    SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, 4);
                    SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MAJOR_VERSION, 3);
                    SDL_GL_SetAttribute(SDL_GLattr.SDL_GL_CONTEXT_MINOR_VERSION, 1);*/

                    window = SDL_CreateWindow(GameInfo.GetWithHighestPriority().Game, SDL_WINDOWPOS_UNDEFINED, SDL_WINDOWPOS_UNDEFINED, windowSize.x, windowSize.y, SDL_WindowFlags.SDL_WINDOW_BORDERLESS);
                    if (window == null)
                    {
                        MessageBox.Show("Error while initializing SDL window:\n" + SDL_GetError() + "\nGame launch halted.", "OOC Engine Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        ExitEvent(null, null);
                    }
                    else
                    {
                        //SDL_GL_CreateContext(window);
                        
                        

                        renderer = SDL_CreateRenderer(window, -1, SDL_RendererFlags.SDL_RENDERER_ACCELERATED);
                        if(renderer == null)
                        {
                            Debug.Log(SDL_GetError(), "Render Error", ConsoleColor.Red);
                            File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "latest.log"), Debug.z_log);
                            Application.Exit();
                        }
                        else
                        {
                            SDL_SetRenderDrawColor(renderer, Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(00), Convert.ToByte(255));
                            IMG_Init(IMG_InitFlags.IMG_INIT_PNG);
                            Mix_OpenAudio(44100, MIX_DEFAULT_FORMAT, 5, 2048);
                            TTF_Init();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.ToString(), "Unhandled Error", ConsoleColor.Red);
                MessageBox.Show("An exception occured on layer LowLevel! Cannot continue.", "OOC Engine Low Level Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "latest.log"), Debug.z_log);
                Application.Exit();
            }
        }

        /*public unsafe static void initGL()
        {
            gProgramID = GL.CreateProgram();

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            string vertexShaderSource = OOCUtil.LoadResource("shader", "vertex.txt");

            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);
            GL.AttachShader(gProgramID, vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            string fragmentShaderSource = OOCUtil.LoadResource("shader", "fragment.txt");

            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);
            GL.AttachShader(gProgramID, fragmentShader);

            gVertexPos2DLocation = GL.GetAttribLocation(gProgramID, "LVertexPos2D");
            GL.ClearColor(1f, 1f, 1f, 1f);

            float[] vertexData =
            {
                -0.5f, -0.5f,
                0.5f, -0.5f,
                0.5f,  0.5f,
                -0.5f,  0.5f
            };
            int[] indexData =
            {
                0, 1, 2, 3
            };
            GL.GenBuffers(1, (int*)gVBO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, gVBO);
            GL.BufferData(BufferTarget.ArrayBuffer, 2 * 4 * sizeof(float), vertexData, BufferUsageHint.StaticDraw);

            GL.GenBuffers(1, (int*)gIBO);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, gIBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, 4 * sizeof(uint), indexData, BufferUsageHint.StaticDraw);

        }*/

        public static void drawevent()
        {
            gameHandlerInstance.Draw();
        }

        static void ExitEvent(object sender, EventArgs e)
        {
            Debug.Log("Exiting out of the game...");
            SDL_DestroyWindow(window);
            SDL_Quit();
            File.WriteAllLines(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "latest.log"), Debug.z_log);
        }
    }
}
namespace OOCEngine
{
    public struct Vector2px
    {
        public int x { get; set; }
        public int y { get; set; }

        public Vector2px(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }
    }

    public static class Debug
    {
        public static List<string> z_log = new List<string>();

        public static void Log(string text, string from = "OOC Engine", ConsoleColor fromColor = ConsoleColor.Green)
        {
            z_log.Add("[" + from + "] " + text);
            Console.Write("[");
            Console.ForegroundColor = fromColor;
            Console.Write(from);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("] " + text);
        }
    }

    public struct Vector2
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vector2(int xPos, int yPos)
        {
            x = xPos;
            y = yPos;
        }
    }

    public struct Rect
    {
        public int x { get; set; }
        public int y { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public Rect(int xPos, int yPos, int w, int h)
        {
            x = xPos;
            y = yPos;
            width = w;
            height = h;
        }

        public SDL_Rect toSDL()
        {
            SDL_Rect sdl = new SDL_Rect();
            sdl.x = x; sdl.y = y;
            sdl.w = width;
            sdl.h = height;
            return sdl;
        }
    }
}