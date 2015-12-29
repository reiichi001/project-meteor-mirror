using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.Actor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public CharaWork charaWork = new CharaWork();
        public PlayerWork playerWork = new PlayerWork();

        public uint displayNameID = 0xFFFFFFFF;
        public string customDisplayName;

        public uint modelID;
        public uint[] appearanceIDs = new uint[0x1D];

        public uint currentTarget = 0xC0000000;
        public uint currentLockedTarget = 0xC0000000;

        public float positionX, positionY, positionZ, rotation;
        public float oldPositionX, oldPositionY, oldPositionZ, oldRotation;
        public ushort moveState, oldMoveState;

        public uint currentState = SetActorStatePacket.STATE_PASSIVE;

        public uint currentZoneID;       

        public Actor(uint id)
        {
            actorID = id;
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
            return SetActorPositionPacket.buildPacket(actorID, playerActorID, SetActorPositionPacket.INNPOS_X, SetActorPositionPacket.INNPOS_Y, SetActorPositionPacket.INNPOS_Z, SetActorPositionPacket.INNPOS_ROT, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
            //return SetActorPositionPacket.buildPacket(actorID, playerActorID, -211.895477f, 190.000000f, 29.651011f, 2.674819f, SetActorPositionPacket.SPAWNTYPE_PLAYERWAKE);
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
            subpackets.Add(createSpeedPacket(playerActorID));
            subpackets.Add(createSpawnPositonPacket(playerActorID, 0xFF));
            subpackets.Add(createAppearancePacket(playerActorID));
            subpackets.Add(createNamePacket(playerActorID));
            subpackets.Add(_0xFPacket.buildPacket(playerActorID, playerActorID));
            subpackets.Add(createStatePacket(playerActorID));
            //subpackets.Add(createScriptBindPacket(playerActorID));                        
            return BasePacket.createPacket(subpackets, true, false);
        }

        public List<SubPacket> createInitSubpackets(uint playerActorID)
        {
            List<SubPacket> subpacketList = new List<SubPacket>();
            SetActorPropetyPacket setProperty = new SetActorPropetyPacket();

            setProperty.addByte(0x0DB5A5BF, 5);
            setProperty.addProperty(this, "charaWork.battleSave.potencial");
            setProperty.addProperty(this, "charaWork.property[0]");
            setProperty.addProperty(this, "charaWork.property[1]");
            setProperty.addProperty(this, "charaWork.property[2]");
            setProperty.addProperty(this, "charaWork.property[4]");
            setProperty.addProperty(this, "charaWork.parameterSave.hp[0]");
            setProperty.addProperty(this, "charaWork.parameterSave.hpMax[0]");
            setProperty.addProperty(this, "charaWork.parameterSave.mp");
            setProperty.addProperty(this, "charaWork.parameterSave.mpMax");
            setProperty.addProperty(this, "charaWork.parameterTemp.tp");
            
            setProperty.addProperty(this, "charaWork.parameterSave.state_mainSkill[0]");
            setProperty.addProperty(this, "charaWork.parameterSave.state_mainSkillLevel");
            setProperty.addProperty(this, "charaWork.depictionJudge");
            setProperty.addProperty(this, "charaWork.statusShownTime[0]");

            setProperty.setTarget("/_init");

            subpacketList.Add(setProperty.buildPacket(actorID, playerActorID));

            return subpacketList;
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
