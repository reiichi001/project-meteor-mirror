using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class Actor
    {
        enum Appearance {
            MODEL,
            SIZE,
            COLORINFO,
            FACEINFO,
            HIGHLIGHT_HAIR,
            VOICE,
            WEAPON1,
            WEAPON2,
            WEAPON3,
            HEADGEAR,
            BODYGEAR,
            LEGSGEAR,
            HANDSGEAR,
            FEETGEAR,
            WAISTGEAR,
            UNKNOWN1,
            R_EAR,
            L_EAR,
            UNKNOWN2,
            UNKNOWN3,
            R_FINGER,
            L_FINGER
        }

        public uint actorID;

        public uint displayNameID;
        public string customDisplayName;

        public uint[] appearanceIDs;

        public float positionX, positionY, positionZ;
        public float oldPositionX, oldPositionY, oldPositionZ;
        public float rotation;
        public ushort moveState;

        public Actor(uint id)
        {
            actorID = id;
        }

    }
}
