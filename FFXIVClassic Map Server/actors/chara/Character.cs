using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class Character:Actor
    {
        public const int SIZE = 0;
        public const int COLORINFO = 1;
        public const int FACEINFO = 2;
        public const int HIGHLIGHT_HAIR = 3;
        public const int VOICE = 4;
        public const int WEAPON1 = 5;
        public const int WEAPON2 = 6;
        public const int WEAPON3 = 7;
        public const int UNKNOWN1 = 8;
        public const int UNKNOWN2 = 9;
        public const int UNKNOWN3 = 10;
        public const int UNKNOWN4 = 11;
        public const int HEADGEAR = 12;
        public const int BODYGEAR = 13;
        public const int LEGSGEAR = 14;
        public const int HANDSGEAR = 15;
        public const int FEETGEAR = 16;
        public const int WAISTGEAR = 17;
        public const int UNKNOWN5 = 18;
        public const int R_EAR = 19;
        public const int L_EAR = 20;
        public const int UNKNOWN6 = 21;
        public const int UNKNOWN7 = 22;
        public const int R_FINGER = 23;
        public const int L_FINGER = 24;

        public uint modelID;
        public uint[] appearanceIDs = new uint[0x1D];

        public uint currentTarget = 0xC0000000;
        public uint currentLockedTarget = 0xC0000000;

        public uint currentMainState = SetActorStatePacket.MAIN_STATE_PASSIVE;
        public uint currentSubState = SetActorStatePacket.SUB_STATE_PLAYER;

        public CharaWork charaWork = new CharaWork();
        public PlayerWork playerWork = new PlayerWork();

        public Character(uint actorID) : base(actorID)
        {
        }

        public SubPacket createAppearancePacket(uint playerActorID)
        {
            SetActorAppearancePacket setappearance = new SetActorAppearancePacket(modelID, appearanceIDs);
            return setappearance.buildPacket(actorId, playerActorID);
        }

        public SubPacket createStatePacket(uint playerActorID)
        {
            return SetActorStatePacket.buildPacket(actorId, playerActorID, currentMainState, currentSubState);
        }

        public BasePacket createActorSpawnPackets(uint playerActorID)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createSpeedPacket(playerActorID));
            subpackets.Add(createSpawnPositonPacket(playerActorID, 0xFF));
            subpackets.Add(createAppearancePacket(playerActorID));
            subpackets.Add(createNamePacket(playerActorID));
            subpackets.Add(_0xFPacket.buildPacket(playerActorID, playerActorID));
            subpackets.Add(createStatePacket(playerActorID));            
            return BasePacket.createPacket(subpackets, true, false);
        }

    }

}
