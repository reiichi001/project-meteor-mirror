using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.Actor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class Actor
    {        
        public const int    SIZE = 0;
        public const int    COLORINFO = 1;
        public const int    FACEINFO = 2;
        public const int    HIGHLIGHT_HAIR = 3;
        public const int    VOICE = 4;
        public const int    WEAPON1 = 5;
        public const int    WEAPON2 = 6;
        public const int    WEAPON3 = 7;
        public const int    UNKNOWN1 = 8;
        public const int    UNKNOWN2 = 9;
        public const int    UNKNOWN3 = 10;
        public const int    UNKNOWN4 = 11;
        public const int    HEADGEAR = 12;
        public const int    BODYGEAR = 13;
        public const int    LEGSGEAR = 14;
        public const int    HANDSGEAR = 15;
        public const int    FEETGEAR = 16;
        public const int    WAISTGEAR = 17;
        public const int    UNKNOWN5 = 18;
        public const int    R_EAR = 19;
        public const int    L_EAR = 20;
        public const int    UNKNOWN6 = 21;
        public const int    UNKNOWN7 = 22;
        public const int    R_FINGER = 23;
        public const int    L_FINGER = 24;

        public uint actorID;

        public bool isNameplateVisible;
        public bool isTargetable;

        public uint displayNameID = 0xFFFFFFFF;
        public string customDisplayName;
        public uint nameplateIcon;

        public uint modelID;
        public uint[] appearanceIDs = new uint[0x1D];

        public uint currentTarget = 0xC0000000;
        public uint currentLockedTarget = 0xC0000000;

        public float positionX, positionY, positionZ, rotation;
        public float oldPositionX, oldPositionY, oldPositionZ, oldRotation;
        public ushort moveState, oldMoveState;

        public uint currentState = SetActorStatePacket.STATE_PASSIVE;

        public uint currentZoneID;

        //Properties
        public ushort curHP, curMP, curTP;
        public ushort maxHP, maxMP, maxTP;

        public byte currentJob;
        public ushort currentLevel;

        public ushort statSTR, statVIT, statDEX, statINT, statMIN, statPIE;
        public ushort statAttack, statDefense, statAccuracy, statAttackMgkPotency, statHealingMgkPotency, statEnchancementMgkPotency, statEnfeeblingMgkPotency, statMgkAccuracy, statMgkEvasion;
        public ushort resistFire, resistIce, resistWind, resistEarth, resistLightning, resistWater;

        public uint currentEXP;

        public ushort linkshellcrest;

        public Actor(uint id)
        {
            actorID = id;
        }

        public void setPlayerAppearance()
        {
            Appearance appearance = Database.getAppearance(actorID);

            modelID = Appearance.getTribeModel(appearance.tribe);
            appearanceIDs[SIZE] = appearance.size;
            appearanceIDs[COLORINFO] = (uint)(appearance.skinColor | (appearance.hairColor << 10) | (appearance.eyeColor << 20));
            appearanceIDs[FACEINFO] = PrimitiveConversion.ToUInt32(appearance.getFaceInfo());
            appearanceIDs[HIGHLIGHT_HAIR] = (uint)(appearance.hairHighlightColor | appearance.hairStyle << 10);
            appearanceIDs[VOICE] = appearance.voice;
            appearanceIDs[WEAPON1] = appearance.mainHand;
            appearanceIDs[WEAPON2] = appearance.offHand;
            appearanceIDs[HEADGEAR] = appearance.head;
            appearanceIDs[BODYGEAR] = appearance.body;
            appearanceIDs[LEGSGEAR] = appearance.legs;
            appearanceIDs[HANDSGEAR] = appearance.hands;
            appearanceIDs[FEETGEAR] = appearance.feet;
            appearanceIDs[WAISTGEAR] = appearance.waist;
            appearanceIDs[R_EAR] = appearance.rightEar;
            appearanceIDs[L_EAR] = appearance.leftEar;
            appearanceIDs[R_FINGER] = appearance.rightFinger;
            appearanceIDs[L_FINGER] = appearance.leftFinger;
        }

        public SubPacket createNamePacket(uint playerActorID)
        {            
            return SetActorNamePacket.buildPacket(actorID, playerActorID,  displayNameID, displayNameID == 0xFFFFFFFF ? customDisplayName : "");
        }

        public SubPacket createAppearancePacket(uint playerActorID)
        {
            SetActorAppearancePacket setappearance = new SetActorAppearancePacket(modelID, appearanceIDs);
            return setappearance.buildPacket(actorID, playerActorID);
        }

        public SubPacket createStatePacket(uint playerActorID)
        {
            return SetActorStatePacket.buildPacket(actorID, playerActorID,  currentState);
        }

        public SubPacket createSpeedPacket(uint playerActorID)
        {
            return SetActorSpeedPacket.buildPacket(actorID, playerActorID);
        }

        public SubPacket createSpawnPositonPacket(uint playerActorID, uint spawnType)
        {
            return SetActorPositionPacket.buildPacket(actorID, playerActorID,  positionX, positionY, positionZ, rotation, spawnType);
        }

        public SubPacket createPositionUpdatePacket(uint playerActorID)
        {
            return MoveActorToPositionPacket.buildPacket(actorID, playerActorID, positionX, positionY, positionZ, rotation, moveState);
        }        

        public SubPacket createScriptBindPacket(uint playerActorID)
        {
            return null;
        }

        public BasePacket createActorSpawnPackets(uint playerActorID)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(AddActorPacket.buildPacket(actorID, playerActorID, 8));
            subpackets.Add(createSpeedPacket(playerActorID));
            subpackets.Add(createSpawnPositonPacket(playerActorID, 0));
            subpackets.Add(createAppearancePacket(playerActorID));
            subpackets.Add(createNamePacket(playerActorID));
            subpackets.Add(createStatePacket(playerActorID));
            //subpackets.Add(createScriptBindPacket(playerActorID));
            subpackets.Add(new SubPacket(0xCC, actorID, playerActorID,  File.ReadAllBytes("./packets/playerscript")));
            subpackets.Add(new SubPacket(0x137, actorID, playerActorID, File.ReadAllBytes("./packets/playerscript2")));
            subpackets.Add(new SubPacket(0x137, actorID, playerActorID, File.ReadAllBytes("./packets/playerscript3")));
            return BasePacket.createPacket(subpackets, true, false);
        }

        public override bool Equals(Object obj)
        {
            Actor actorObj = obj as Actor;
            if (actorObj == null)
                return false;
            else
                return actorID == actorObj.actorID;
        }

    }
}
