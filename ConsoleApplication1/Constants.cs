using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{

    /// <summary>
    /// This class contains global contants for use primarily in the state logic
    /// </summary>
    class Constants
    {
        //Constants relating to the indecies of the state variable
        public const int AIR_STATE = 0;
        public const int GRAB_STATE = 1;
        public const int DASH_STATE = 2;
        public const int COLLISION_STATE = 3;

        //Constants for air states
        public const int ON_GROUND = 0;
        public const int FIRST_JUMP = 1;
        public const int CHARGE_READY = 2;
        public const int SECOND_JUMP = 3;
        public const int NO_CHARGE = 4;

        //Constants for grab states
        public const int GRAB_NONE = 0;
        public const int GRAB_TOP = 1;
        public const int GRAB_LEFT = 2;
        public const int GRAB_RIGHT = 3;
        public const int GRAB_ENEMY = 4;

        //Constants for dash states
        public const int NO_DASH = -1;
        public const int DASH_READY = 0;

        //Constants for collision states
        public const int COLLIDE_NONE = 0;
        public const int COLLIDE_TOP = 1;
        public const int COLLIDE_LEFT = 2;
        public const int COLLIDE_RIGHT = 3;
        public const int COLLIDE_BOTTOM = 4;
    }
}
