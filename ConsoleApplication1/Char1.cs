using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace ConsoleApplication1
{
    /// <summary>
    /// The first type of character, extends the Player class
    /// </summary>
    class Char1: Player
    {

        /// <summary>
        /// Constructor for Char1, sets all of the character dependant constants
        /// </summary>
        public Char1()
        {
            // Character constants
            charSize = new Vector2d(25, 25); //Size (width and length) of the character's hitbox
            acceleration = 0.75; //The running acceleration of the character
            runSpeed = 10; //The maximum run speed of the character
            dashSpeed = 11; //The speed of the character's dash
            fallSpeed = 12; //The maximum fall speed of the character
            slideSpeed = 0.5; //The maximum sliding speed of the character when grabbing a wall
            airDrag = 0; //The acceleration of air friction on the character
            wallDrag = 1; //The acceleration of friction on the character when grabbing a wall
            jumpSpeed = 10; //The vertical speed of the character's jumps
            dashLength = 15; //Duration of the character's dash (in frames)

            // Environmental constants
            gravity = 0.5; //Gravitational acceleration on the character, affects the "floatyness"
            
            //In game variables
            position = new Vector2d(0, 0); //Position of the player
            velocity = new Vector2d(0, 0); //Velocity of the player in X (positive to the left) and Y (positive up)
            state = new int[4] { Constants.CHARGE_READY, Constants.GRAB_NONE, 0, Constants.COLLIDE_NONE }; //Array of integers describing the state of the character. the states and indecies are defined as constants in the Constants class
        }

    }
}
