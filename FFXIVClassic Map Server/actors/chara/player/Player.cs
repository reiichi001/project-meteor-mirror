using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects.database;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send.actor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class Player : Character
    {

        public const int TIMER_TOTORAK = 0;
        public const int TIMER_DZEMAEL = 1;        
        public const int TIMER_BOWL_OF_EMBERS_HARD = 2;
        public const int TIMER_BOWL_OF_EMBERS = 3;
        public const int TIMER_THORNMARCH = 4;
        public const int TIMER_AURUMVALE = 5;
        public const int TIMER_CUTTERSCRY = 6;
        public const int TIMER_BATTLE_ALEPORT = 7;
        public const int TIMER_BATTLE_HYRSTMILL = 8;
        public const int TIMER_BATTLE_GOLDENBAZAAR = 9;
        public const int TIMER_HOWLING_EYE_HARD = 10;
        public const int TIMER_HOWLING_EYE = 11;
        public const int TIMER_CASTRUM_TOWER = 12;
        public const int TIMER_BOWL_OF_EMBERS_EXTREME = 13;
        public const int TIMER_RIVENROAD = 14;
        public const int TIMER_RIVENROAD_HARD = 15;
        public const int TIMER_BEHEST = 16;
        public const int TIMER_COMPANYBEHEST = 17;
        public const int TIMER_RETURN = 18;
        public const int TIMER_SKIRMISH = 19;

        public uint[] timers = new uint[20];

        PlayerWork playerWork = new PlayerWork();

        public Player(uint actorID) : base(actorID)
        {
            actorName = String.Format("_pc{0:00000000}", actorID);
            className = "Player";
            currentSubState = SetActorStatePacket.SUB_STATE_PLAYER;

            DBStats stats = Database.getCharacterStats(actorID);

            charaWork.property[0] = 1;
            charaWork.property[1] = 1;
            charaWork.property[2] = 1;
            charaWork.property[4] = 1;            

            charaWork.parameterSave.hp[0] = stats.hp;
            charaWork.parameterSave.hpMax[0] = stats.hpMax;
            charaWork.parameterSave.mp = stats.mp;
            charaWork.parameterSave.mpMax = stats.mpMax;

            charaWork.parameterSave.state_mainSkill[0] = 3;
            charaWork.parameterSave.state_mainSkillLevel = 1;

            charaWork.battleSave.skillLevel = 1;
            charaWork.battleSave.skillLevelCap = 2;
            charaWork.battleSave.potencial = 0.5f;
            charaWork.battleSave.physicalExp = 1;
            charaWork.battleSave.negotiationFlag[0] = false;
            charaWork.battleSave.negotiationFlag[1] = false;

            for (int i = 0; i < 20; i++)
                charaWork.statusShownTime[i] = 0xFFFFFFFF;
            
            setPlayerAppearance();
        }

        public void setPlayerAppearance()
        {
            DBAppearance appearance = Database.getAppearance(true, actorId);

            modelId = DBAppearance.getTribeModel(appearance.tribe);
            appearanceIds[SIZE] = appearance.size;
            appearanceIds[COLORINFO] = (uint)(appearance.skinColor | (appearance.hairColor << 10) | (appearance.eyeColor << 20));
            appearanceIds[FACEINFO] = PrimitiveConversion.ToUInt32(appearance.getFaceInfo());
            appearanceIds[HIGHLIGHT_HAIR] = (uint)(appearance.hairHighlightColor | appearance.hairStyle << 10);
            appearanceIds[VOICE] = appearance.voice;
            appearanceIds[WEAPON1] = appearance.mainHand;
            appearanceIds[WEAPON2] = appearance.offHand;
            appearanceIds[HEADGEAR] = appearance.head;
            appearanceIds[BODYGEAR] = appearance.body;
            appearanceIds[LEGSGEAR] = appearance.legs;
            appearanceIds[HANDSGEAR] = appearance.hands;
            appearanceIds[FEETGEAR] = appearance.feet;
            appearanceIds[WAISTGEAR] = appearance.waist;
            appearanceIds[R_EAR] = appearance.rightEar;
            appearanceIds[L_EAR] = appearance.leftEar;
            appearanceIds[R_FINGER] = appearance.rightFinger;
            appearanceIds[L_FINGER] = appearance.leftFinger;
        }

        public List<SubPacket> create0x132Packets(uint playerActorId)
        {
            List<SubPacket> packets = new List<SubPacket>();
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0xB, "commandForced"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0xA, "commandDefault"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x6, "commandWeak"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x4, "commandContent"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x6, "commandJudgeMode"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x100, "commandRequest"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x100, "widgetCreate"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x100, "macroRequest"));
            return packets;
        }

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            if (isMyPlayer(playerActorId))
            {
                lParams = LuaUtils.createLuaParamList("/Chara/Player/Player_work", false, false, false, true, 0, false, timers, true);
            }
            else
                lParams = LuaUtils.createLuaParamList("/Chara/Player/Player_work", false, false, false, false, false, true);
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);
        }        

        public override BasePacket getInitPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId));
            if (isMyPlayer(playerActorId))
                subpackets.AddRange(create0x132Packets(playerActorId));
            subpackets.Add(createSpeedPacket(playerActorId));
            subpackets.Add(createSpawnPositonPacket(playerActorId, 0x1));
            subpackets.Add(createAppearancePacket(playerActorId));
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(_0xFPacket.buildPacket(playerActorId, playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIdleAnimationPacket(playerActorId));
            subpackets.Add(createInitStatusPacket(playerActorId));
            subpackets.Add(createSetActorIconPacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));
            subpackets.Add(createScriptBindPacket(playerActorId));

            return BasePacket.createPacket(subpackets, true, false);
        }

        public bool isMyPlayer(uint otherActorId)
        {
            return actorId == otherActorId;
        }        
    }
}
