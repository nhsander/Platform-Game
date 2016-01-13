using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ConsoleApplication1
{
    /// <summary>
    /// This class handles all collision logic
    /// </summary>
    class CollisionDetector
    {
        //Constructor
        public CollisionDetector() { }

        /// <summary>
        /// Check all collisions and collide where necessary
        /// </summary>
        /// <param name="platforms">List of all platforms to check</param>
        /// <param name="player">The player to collide with</param>
        /// <param name="aspectRatio">Describes the ratio of the game window height to width, used because sizes are defined as a fraction of the window size</param>
        public void collisions(List<Platform> platforms, Player player, int width, int height)
        {
            //Assume no collision
            bool hasCollide = false;

            //Assume the player has left the ground
            //If the player remains on the ground, that condition will be handled in the collision
            if (player.state[Constants.AIR_STATE] == Constants.ON_GROUND)
                player.state[Constants.AIR_STATE] = Constants.CHARGE_READY;

            //Check each platform and collide where necessary
            foreach (Platform plat in platforms)
            {
                if (plat.collides(player, width, height))
                {
                    collide(plat, player, width, height); //Collide with rectangle
                    //Console.WriteLine(platforms.IndexOf(plat).ToString());
                    hasCollide = true;
                }
            }

            //If there was no collision, set state accordingly
            if (!hasCollide)
                player.state[Constants.COLLISION_STATE] = Constants.COLLIDE_NONE;

        }

        /// <summary>
        /// Collide a platform with a player
        /// </summary>
        /// <param name="plat">Platform to collide</param>
        /// <param name="player">Player to collide</param>
        /// <param name="aspectRatio">Describes the ratio of the game window height to width, used because sizes are defined as a fraction of the window size</param>
        private void collide(Platform plat, Player player, int width, int height)
        {
            ////Point describing the top left of the player
            //Vector2d amax = new Vector2d( ( player.position.X - player.charSize.X ) / width, ( player.position.Y + player.charSize.Y ) / height);
            ////Point describing the bottom right of the player
            //Vector2d amin = new Vector2d( ( player.position.X + player.charSize.X ) / width, ( player.position.Y - player.charSize.Y ) / height);

            ////Point describing the top left of the platform
            //Vector2d bmax = new Vector2d( ( plat.position.X - plat.platSize.X ) / width, ( plat.position.Y + plat.platSize.Y ) / height);
            ////Point describing the bottom right of the player
            //Vector2d bmin = new Vector2d( ( plat.position.X + plat.platSize.X ) / width, ( plat.position.Y - plat.platSize.Y ) / height);

            //Point describing the top left of the player
            Vector2d amax = new Vector2d((player.position.X + player.charSize.X), (player.position.Y - player.charSize.Y));
            //Point describing the bottom right of the player
            Vector2d amin = new Vector2d((player.position.X - player.charSize.X), (player.position.Y + player.charSize.Y));

            //Point describing the top left of the platform
            Vector2d bmax = new Vector2d((plat.position.X + plat.platSize.X), (plat.position.Y - plat.platSize.Y));
            //Point describing the bottom right of the player
            Vector2d bmin = new Vector2d((plat.position.X - plat.platSize.X), (plat.position.Y + plat.platSize.Y));

            //Declare vector for displacement
            Vector2d mtd = new Vector2d(0, 0);

            //displacements in each direction
            double right = bmin.X - amax.X;
            double left = bmax.X - amin.X;
            double bottom = bmin.Y - amax.Y;
            double top = bmax.Y - amin.Y;

            //The minimum displacement is the collision direction
            double min = Math.Min(Math.Min(Math.Abs(left), Math.Abs(right)), Math.Min(Math.Abs(top), Math.Abs(bottom)));

            //check collisions in each direction
            if (Math.Abs(left) == min)
            {
                mtd.X = left;
                collideLeft(player);
            }
            if (Math.Abs(right) == min)
            {
                mtd.X = right;
                collideRight(player);
            }
            if (Math.Abs(top) == min)
            {
                mtd.Y = top;
                collideTop(player);
            }
            if (Math.Abs(bottom) == min)
            {
                mtd.Y = bottom;
                collideBottom(player);
            }

            player.position += mtd;

        }
        
        /// <summary>
        /// collide with a platform to the left
        /// </summary>
        /// <param name="player"></param>
        private void collideLeft(Player player)
        {
            if (player.velocity.X < 0)
                player.velocity.X = 0;
            
            player.state[Constants.COLLISION_STATE] = Constants.COLLIDE_LEFT;
        }

        /// <summary>
        /// collide with a platform to the right
        /// </summary>
        /// <param name="player"></param>
        private void collideRight(Player player)
        {
            if (player.velocity.X > 0)
                player.velocity.X = 0;

            player.state[Constants.COLLISION_STATE] = Constants.COLLIDE_RIGHT;
        }

        /// <summary>
        /// collide with a platform to the top
        /// </summary>
        /// <param name="player"></param>
        private void collideTop(Player player)
        {
            if (player.velocity.Y > 0)
                player.velocity.Y = 0;

            player.state[Constants.COLLISION_STATE] = Constants.COLLIDE_TOP;
        }

        /// <summary>
        /// collide with a platform to the bottom
        /// </summary>
        /// <param name="player"></param>
        private void collideBottom(Player player)
        {
            if (player.velocity.Y < 0)
                player.velocity.Y = 0;

            player.state[Constants.COLLISION_STATE] = Constants.COLLIDE_BOTTOM;
            player.state[Constants.AIR_STATE] = Constants.ON_GROUND;
            player.state[Constants.GRAB_STATE] = Constants.GRAB_NONE;
        }


    }
}
