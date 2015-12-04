using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class PlayerActor : Actor
    {
        public PlayerActor(uint actorID) : base(actorID)
        {
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
            DBAppearance appearance = Database.getAppearance(actorID);

            modelID = DBAppearance.getTribeModel(appearance.tribe);
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
    }
}
