using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GrappleGame
{
    class Constants
    {
        public const int tilesize = 32;

        /// <summary>
        /// Used to Handle All Character Generated Events
        /// </summary>
        /// <param name="EventID">EventID = 0 (Character Requesting To Walk)
        /// EventID = 1 (Character Requesting to be Deleted)
        /// EventID = 2 (Dude Next to and facing a Character)
        /// EventID = 3 (Dude Wants to Talk to the Character)</param>
        /// <param name="Input">Dependent Upon the EventID Number See Method EventID Calls</param>
        /// <param name="ID">List ID index on the Current Map for Character List</param>
        /// <returns></returns>
        public delegate bool CharacterEventHandler(int EventID, string Input, int ID);
        public Constants()
        {
        }
    }
}
