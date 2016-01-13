using System;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;


namespace ConsoleApplication1
{
    class Game: GameWindow
    {

        private Color currentColor = Color.AntiqueWhite; //Background color
        private Player player; //variable for the player character
        private List<Platform> platforms; //array of all platforms in the level
        private CollisionDetector collisionDetector = new CollisionDetector();

        List<Vector2d> Vertices; //Used in rendering

        /// <summary>
        /// This method overrides the base OnLoad method from GameWindow
        /// 
        /// It runs when the game is first loaded
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            
            //Set the title of the game
            Title = "Game!";

            //Make a character object and assign it to the player variable
            player = new Char1();

            //Load the level
            platforms = loadPlatforms();
            
            //Set the background color of the window
            GL.ClearColor(currentColor);

        }

        /// <summary>
        /// This method overrides the base OnRenderFrame method from GameWindow
        /// 
        /// It renders each frame
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            //OpenGL graphics initialization
            GL.ClearColor(currentColor);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);

            //Setup OpenGL graphics to render the player
            GL.Begin(PrimitiveType.Polygon);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.Color3(0, 0, 0);

            //Get vertices of the player
            Vertices = player.render(Width, Height);

            //Render the player
            foreach (Vector2d vertex in Vertices)
            {
                GL.Vertex2(vertex);
            }

            //End rendering the player
            GL.End();


            //Loop through all of the platforms to render them
            foreach (Platform plat in platforms)
            {

                //Begin GL graphics to render the current platform
                GL.Begin(PrimitiveType.Polygon);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.Color3(0.3, 0.3, 0.3);

                //Ger vertices for rendering the current platform
                Vertices = plat.render(Width, Height);

                //Render the platform
                foreach (Vector2d vertex in Vertices)
                    GL.Vertex2(vertex);

                //End rendering the platform
                GL.End();
            }

            //Update the window
            SwapBuffers();

        }

        /// <summary>
        /// This method overrides the base OnUpdateFrame method from GameWindow
        /// 
        /// It updates each frame, and contains the majority of the game logic
        /// </summary>
        /// <param name="e"></param>
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            //Trigger player movement
            player.movement(this);

            //Run collision detection
            collisionDetector.collisions(platforms, player, Width, Height);
        }

        /// <summary>
        /// This method overrides the base OnResize method from GameWindow
        /// 
        /// It runs when the game window is resized
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        private List<Platform> loadPlatforms()
        {
            //Initialize platform list
            List<Platform> plats = new List<Platform>();

            //Declare variables for the platforms that will be read from the file
            Vector2d Center = new Vector2d();
            Vector2d Size = new Vector2d();
            Platform plat;

            //Declare variables to be used while the file is being read
            string line;
            char[] array;
            int count = 0;
            string subStr = "";

            //Initialize the file reader
            System.IO.StreamReader file = new System.IO.StreamReader("Levels/level.txt");

            //Read each line of the file
            while ((line = file.ReadLine()) != null)
            {
                //Reset to beginning of the line
                subStr = "";
                count = 0;
                array = line.ToCharArray();
                Center = new Vector2d();
                Size = new Vector2d();

                //Parse through each character
                foreach (char c in array)
                {
                    //Add the next character to the substring if it is not a space
                    if (c != ' ')
                        subStr += c;

                    //When the next space is reached, use the substring to set the next value of the platform
                    if (c == ' ')
                    {
                        if (count == 0)
                            Center.X = Double.Parse(subStr);
                        if (count == 1)
                            Center.Y = Double.Parse(subStr);
                        if (count == 2)
                            Size.X = Double.Parse(subStr);
                        if (count == 3)
                            Size.Y = double.Parse(subStr);
                        subStr = "";
                        count++;
                    }
                }

                //Create the platform and add it to the list
                plat = new Platform(Center, Size);
                plats.Add(plat);

            }

            //return the platform list
            return plats;
        }

    }
}
