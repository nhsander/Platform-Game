using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ConsoleApplication1
{
    /// <summary>
    /// This class describes and a single rectangular platform
    /// </summary>
    class Platform
    {

        public Vector2d position; //Position of the center of the platform
        public Vector2d platSize; //Size in width and height of the platform

        /// <summary>
        /// platform constructor
        /// </summary>
        /// <param name="p">Position of the center of the platform</param>
        /// <param name="s">Size in width and height of the platform</param>
        public Platform(Vector2d p, Vector2d s)
        {
            position = p;
            platSize = s;
        }

        /// <summary>
        /// Returns a list of points to draw the platform to the game window.
        /// </summary>
        /// <param name="width">Width of the window. Used to define relative window position</param>
        /// <param name="height">Height of the window. Used to define relative window position</param>
        /// <returns>list of Vector2d</returns>
        public List<Vector2d> render(int width, int height)
        {
            List<Vector2d> vertices = new List<Vector2d>();

            vertices.Add(new Vector2d(( position.X - platSize.X ) / width, ( position.Y + platSize.Y ) / height));
            vertices.Add(new Vector2d(( position.X + platSize.X ) / width, ( position.Y + platSize.Y ) / height));
            vertices.Add(new Vector2d(( position.X + platSize.X ) / width, ( position.Y - platSize.Y ) / height));
            vertices.Add(new Vector2d(( position.X - platSize.X ) / width, ( position.Y - platSize.Y ) / height));

            return vertices;
        }

        /// <summary>
        /// Used to check collision with the player, returns true of this platform and the palyer collide
        /// </summary>
        /// <param name="player">the Player</param>
        /// <returns>returns true of this platform and the palyer collide</returns>
        public bool collides(Player player, int width, int height)
        {

            //platform corners, Min is bottom left and max is top right
            Vector2d platMin = position - platSize;
            Vector2d platMax = position + platSize;

            //player corners, Min is bottom left and max it top right
            Vector2d playerMin = player.position + player.velocity - player.charSize;
            Vector2d playerMax = player.position + player.velocity + player.charSize;

            //if statement checks the sides of the platform compared to the sides of the player
            if (platMin.X > playerMax.X || //if the left of the platform is further right than the right side of the player or
                platMin.Y > playerMax.Y || //the bottom of the platform is higher than the top of the player or
                platMax.X < playerMin.X || //the right of the platform is further left than the left of the player
                platMax.Y < playerMin.Y)   //the top of the platform is lower than the bottom of the player
            {
                return false;  //then there is no collision, so return false
            }
            return true; //otherwise, there is a collision, so return true
            }

    }
}
