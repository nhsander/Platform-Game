using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace ConsoleApplication1
{

    /// <summary>
    /// This class is a player controlled object, currently represented by a rectangle on screen.
    /// </summary>
    class Player
    {
        //// Character dependant constants

        // Character constants
        public Vector2d charSize; //Size (width and length) of the character's hitbox
        public double acceleration; //The running acceleration of the character
        public double runSpeed; //The maximum run speed of the character
        public double dashSpeed; //The speed of the character's dash
        public double fallSpeed; //The default fall speed of the character
        public double slideSpeed; //The maximum sliding speed of the character when grabbing a wall
        public double airDrag; //The acceleration of air friction on the character
        public double wallDrag; //The acceleration of friction on the character when grabbing a wall
        public double jumpSpeed; //The vertical speed of the character's jumps
        public int dashLength; //The duration of the character's dash (in frames)

        // Environmental constants
        public double gravity; //Gravitational acceleration on the character, affects the "floatyness"

        //// End character dependant constants

        //In game variables
        public Vector2d position; //Position of the player
        public Vector2d velocity; //Velocity of the player in X (positive to the left) and Y (positive up)
        public int[] state; //Array of integers describing the state of the character. the states and indecies are defined as constants in the Constants class

        /// <summary>
        /// Returns a list of points to draw the character to the game window.
        /// </summary>
        /// <param name="width">Width of the window. Used to define relative window position</param>
        /// <param name="height">Height of the window. Used to define relative window position</param>
        /// <returns>list of Vector2d</returns>
        public List<Vector2d> render(int width, int height)
        {
            List<Vector2d> vertices = new List<Vector2d>();

            vertices.Add(new Vector2d( (position.X - charSize.X) / width, (position.Y + charSize.Y) / height));
            vertices.Add(new Vector2d( (position.X + charSize.X) / width, (position.Y + charSize.Y) / height));
            vertices.Add(new Vector2d( (position.X + charSize.X) / width, (position.Y - charSize.Y) / height));
            vertices.Add(new Vector2d( (position.X - charSize.X) / width, (position.Y - charSize.Y) / height));

            return vertices;
        }

        /// <summary>
        /// Handles all player movement
        /// Is called on every frame update
        /// </summary>
        public void movement(Game game)
        {
            //Get the current keyboard state
            var keyState = OpenTK.Input.Keyboard.GetState();

            //Call method to handle gravity (and other vertical environmental forces)
            fall();

            //If the user presses escape, close the game
            if (keyState[Key.Escape])
                game.Close();

            //Call the method to handle grab state management
            grab(keyState[Key.Left], keyState[Key.Right], keyState[Key.Up]);

            //Call the method to handle left and right motion, and pass true or false for left and right arrow keys
            move(keyState[Key.Left], keyState[Key.Right]);

            //Call the methods to handle jumping and air state refreshing
            jump(keyState[Key.Space], keyState[Key.Left], keyState[Key.Right]);
            stateRefresh(keyState[Key.Space], keyState[Key.LShift] || keyState[Key.RShift]);

            //Call the method to handle dashing
            dash(keyState[Key.LShift] || keyState[Key.RShift], keyState[Key.Left], keyState[Key.Right], keyState[Key.Down]);

            //Call the method to handle horizontal environmental forces (friction, etc)
            decelerate(keyState[Key.Left], keyState[Key.Right]);

            //Update the character position
            position += velocity;

        }

        //// Methods relating to checking the state

        /// <summary>
        /// This method updates the air state every frame
        /// </summary>
        /// <param name="space">True if the spacebar is pressed</param>
        /// <param name="shift">true if shift is pressed</param>
        private void stateRefresh(bool space, bool shift)
        {
            if (!space && !shift && state[Constants.AIR_STATE] == Constants.FIRST_JUMP)
                state[Constants.AIR_STATE] = Constants.CHARGE_READY;
            if (!space && !shift && state[Constants.AIR_STATE] == Constants.SECOND_JUMP)
                state[Constants.AIR_STATE] = Constants.NO_CHARGE;

            //Decriment the dash counter
            if (state[Constants.DASH_STATE] > 1)
                state[Constants.DASH_STATE]--;
            else if ((state[Constants.DASH_STATE] == 1 || state[Constants.DASH_STATE] == -1) && shift)
                state[Constants.DASH_STATE] = -1;
            else
                state[Constants.DASH_STATE] = 0;
        }

        /// <summary>
        /// This method updates the grab state every frame based on the collision state and the arrow key inputs
        /// </summary>
        /// <param name="left">true if the left arrow key is pressed</param>
        /// <param name="right">true if the right arrow key is pressed</param>
        /// <param name="up">true if the up arrow key is pressed</param>
        private void grab(bool left, bool right, bool up)
        {
            //Assume the player is not grabbing anything
            state[Constants.GRAB_STATE] = Constants.GRAB_NONE;

            //check collision states and arrow keys and assign the correct grab state
            if (state[Constants.COLLISION_STATE] == Constants.COLLIDE_LEFT && left)
                state[Constants.GRAB_STATE] = Constants.GRAB_LEFT;
            if (state[Constants.COLLISION_STATE] == Constants.COLLIDE_RIGHT && right)
                state[Constants.GRAB_STATE] = Constants.GRAB_RIGHT;
            if (state[Constants.COLLISION_STATE] == Constants.COLLIDE_TOP && up)
                state[Constants.GRAB_STATE] = Constants.GRAB_TOP;
            if (state[Constants.COLLISION_STATE] == Constants.COLLIDE_NONE || state[Constants.COLLISION_STATE] == Constants.COLLIDE_BOTTOM)
                state[Constants.GRAB_STATE] = Constants.GRAB_NONE;
        }

        
        //// Methods relating to environmental affects on movement

        /// <summary>
        /// modify the velocity of the player for gravity
        /// </summary>
        private void fall()
        {

            double fallAcceleration = gravity; //Assume the character is falling

            if (state[Constants.DASH_STATE] > 0)
                fallAcceleration = 0; //No falling while the character is dashing

            else if (state[Constants.GRAB_STATE] == Constants.GRAB_TOP ||
                     state[Constants.GRAB_STATE] == Constants.GRAB_ENEMY)
            {
                fallAcceleration = 0; //No falling while grabbing the ceiling or an enemy
                velocity.Y = 0;
            }

            else if (state[Constants.AIR_STATE] == Constants.ON_GROUND)
                fallAcceleration = 0; //No falling while on a surface

            else if ((state[Constants.GRAB_STATE] == Constants.GRAB_LEFT ||
                      state[Constants.GRAB_STATE] == Constants.GRAB_RIGHT) &&
                      velocity.Y >= -slideSpeed)
                fallAcceleration = wallDrag; //Accelerate sliding down the wall when grabbing

            else if ((state[Constants.GRAB_STATE] == Constants.GRAB_LEFT ||
                      state[Constants.GRAB_STATE] == Constants.GRAB_RIGHT) &&
                      velocity.Y < -slideSpeed)
            {
                fallAcceleration = 0;
                velocity.Y = -slideSpeed;
            }

                velocity.Y -= fallAcceleration;
        }

        /// <summary>
        /// This method handles friction relating to horizontal movement
        /// </summary>
        private void decelerate(bool left, bool right)
        {
            //This section will decelerate the player when on the ground
            if (state[Constants.AIR_STATE] == Constants.ON_GROUND && !left && !right)
            {
                if (Math.Abs(velocity.X) > acceleration)
                    velocity.X -= Math.Sign(velocity.X) * acceleration;
                else velocity.X = 0;
            }
            //This section will decelerate the player when in the air
            else if (state[Constants.DASH_STATE] == 0 && !left && !right)
            {
                if (Math.Abs(velocity.X) > airDrag)
                    velocity.X -= Math.Sign(velocity.X) * airDrag;
                else velocity.X = 0;
            }
            //This section will decelerate the player when grabbing the ceiling
            else if (state[Constants.GRAB_STATE] == Constants.GRAB_TOP && !left && !right)
            {
                if (Math.Abs(velocity.X) > wallDrag)
                    velocity.X -= Math.Sign(velocity.X) * wallDrag;
                else velocity.X = 0;
            }
        }

        /// <summary>
        /// This method handles the left and right motion of the character
        /// </summary>
        /// <param name="left">true if the left arrow key is held</param>
        /// <param name="right">true of the right arrow key is held</param>
        private void move(bool left, bool right)
        {
            if (left) //If the left arrow key is held
                if (velocity.X > -runSpeed) //If the character is not at the max run speed
                    velocity.X -= acceleration; //Accelerate to the left
                else //At or above the max run speed
                    velocity.X = -runSpeed; //Set to max run speed
            if (right) //If the right arrow key is held
                if (velocity.X < runSpeed) //If the character is not at the max run speed
                    velocity.X += acceleration; //Accelerate to the right
                else //At or above the max run speed
                    velocity.X = runSpeed; //Set to max run speed
        }

        /// <summary>
        /// This method allows the player to jump
        /// </summary>
        /// <param name="space">true if the space bar is pressed</param>
        /// <param name="left">true if the left arrow key is pressed</param>
        /// <param name="right">true if the right arrow key is pressed</param>
        private void jump(bool space, bool left, bool right)
        {
            // If the spacebar is not held, do nothing
            if (!space)
                return;

            // Jump logic while grabbing the wall
            if (state[Constants.GRAB_STATE] == Constants.GRAB_LEFT) //Jump off of a left wall towards the right
            {
                if (state[Constants.AIR_STATE] == Constants.CHARGE_READY || state[Constants.AIR_STATE] == Constants.NO_CHARGE)
                {
                    state[Constants.GRAB_STATE] = Constants.GRAB_NONE; //Release the wall
                    velocity.Y = jumpSpeed; //Set vertical speed
                    velocity.X = runSpeed; //Jump to the left
                    state[Constants.DASH_STATE] = 0;
                    state[Constants.AIR_STATE] = Constants.FIRST_JUMP; //Reset air charge
                }
            }
            else if (state[Constants.GRAB_STATE] == Constants.GRAB_RIGHT) //Jump off of a right wall towards the left
            {
                if (state[Constants.AIR_STATE] == Constants.CHARGE_READY || state[Constants.AIR_STATE] == Constants.NO_CHARGE)
                {
                    state[Constants.GRAB_STATE] = Constants.GRAB_NONE; //Release the wall
                    velocity.Y = jumpSpeed; //Set vertical speed
                    velocity.X = -runSpeed; //Jump to the left
                    state[Constants.DASH_STATE] = 0;
                    state[Constants.AIR_STATE] = Constants.FIRST_JUMP; //Reset air charge
                }
            }
            // Jump logic while on the ground
            else if (state[Constants.AIR_STATE] == Constants.ON_GROUND)
            {
                // Jump to the left
                if (left && !right)
                {
                    velocity.Y = jumpSpeed;
                    velocity.X = -runSpeed;
                    state[Constants.AIR_STATE] = Constants.FIRST_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }
                // Jump to the right
                else if (!left && right)
                {
                    velocity.Y = jumpSpeed;
                    velocity.X = runSpeed;
                    state[Constants.AIR_STATE] = Constants.FIRST_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }
                // Jump straight up
                else if (Math.Abs(velocity.X) < runSpeed/3)
                {
                    velocity.Y = jumpSpeed;
                    velocity.X = 0;
                    state[Constants.AIR_STATE] = Constants.FIRST_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }
                // Jump without x components
                else
                {
                    velocity.Y = jumpSpeed;
                    state[Constants.AIR_STATE] = Constants.FIRST_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }
            }
            // Jump logic in the air
            else if (state[Constants.AIR_STATE] == Constants.CHARGE_READY)
            {
                // Jump to the left
                if (left && !right)
                {
                    velocity.Y = jumpSpeed;
                    velocity.X = -runSpeed;
                    state[Constants.AIR_STATE] = Constants.SECOND_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }
                // Jump to the right
                else if (!left && right)
                {
                    velocity.Y = jumpSpeed;
                    velocity.X = runSpeed;
                    state[Constants.AIR_STATE] = Constants.SECOND_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }
                // Jump straight up
                else if (Math.Abs(velocity.X) < runSpeed / 3)
                {
                    velocity.Y = jumpSpeed;
                    velocity.X = 0;
                    state[Constants.AIR_STATE] = Constants.SECOND_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }
                // Jump without x components
                else
                {
                    velocity.Y = jumpSpeed;
                    state[Constants.AIR_STATE] = Constants.SECOND_JUMP;
                    state[Constants.DASH_STATE] = 0;
                }

            }

        }

        /// <summary>
        /// This method handles the player dash ability
        /// </summary>
        /// <param name="shift">True if either shift key is pressed</param>
        /// <param name="left">True if the left arrow key is pressed</param>
        /// <param name="right">True if the right arrow key is pressed</param>
        /// <param name="down">True if the down arrow key is pressed</param>
        private void dash(bool shift, bool left, bool right, bool down)
        {
            //No dashing if shift is not held or if no direction is selected
            if (!shift || (!left && !right && !down))
                return;

            //No dashing if the dash key has not been released
            if (state[Constants.DASH_STATE] == -1)
                return;

            //Dash down in the air
            if (state[Constants.AIR_STATE] != Constants.ON_GROUND && down)
            {
                if (velocity.Y > -fallSpeed)
                    velocity.Y = -fallSpeed;
                state[Constants.GRAB_STATE] = Constants.GRAB_NONE;
            }

            //No horizontal dashing if the player is already in a dash
            if (state[Constants.DASH_STATE] > 0)
                return;

            //Dash out from grabbing a wall to the left
            if (state[Constants.GRAB_STATE] == Constants.GRAB_LEFT && state[Constants.AIR_STATE] != Constants.FIRST_JUMP && state[Constants.AIR_STATE] != Constants.SECOND_JUMP)
            {
                state[Constants.GRAB_STATE] = Constants.GRAB_NONE;
                velocity.Y = 0;
                velocity.X = dashSpeed;
                state[Constants.DASH_STATE] = dashLength;
                state[Constants.AIR_STATE] = Constants.FIRST_JUMP;
                return;
            }
            //Dash out from grabbing a wall to the right
            if (state[Constants.GRAB_STATE] == Constants.GRAB_RIGHT && state[Constants.AIR_STATE] != Constants.FIRST_JUMP && state[Constants.AIR_STATE] != Constants.SECOND_JUMP)
            {
                state[Constants.GRAB_STATE] = Constants.GRAB_NONE;
                velocity.Y = 0;
                velocity.X = -dashSpeed;
                state[Constants.DASH_STATE] = dashLength;
                state[Constants.AIR_STATE] = Constants.FIRST_JUMP;
                return;
            }

            //Dash in the air to the left or right
            if (state[Constants.AIR_STATE] == Constants.CHARGE_READY)
            {
                state[Constants.AIR_STATE] = Constants.SECOND_JUMP;
                velocity.Y = 0;
                if (left)
                    velocity.X = -dashSpeed;
                if (right)
                    velocity.X = dashSpeed;
                state[Constants.DASH_STATE] = dashLength;
            }

            //Dash on the ground to the left or right
            if (state[Constants.AIR_STATE] == Constants.ON_GROUND)
            {
                velocity.Y = 0;
                if (left)
                    velocity.X = -dashSpeed;
                if (right)
                    velocity.X = dashSpeed;
                state[Constants.DASH_STATE] = dashLength;
            }


        }

    }

}
