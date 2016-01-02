using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.dataobjects.chara.npc
{
    class Npc : Character
    {
        public Npc(uint id, string actorName, uint displayNameId, string customDisplayName, float positionX, float positionY, float positionZ, float rotation, uint animationId, string className, byte[] initParams)
            : base(id)
        {
            this.actorName = actorName;
            this.displayNameId = displayNameId;
            this.customDisplayName = customDisplayName;
            this.positionX = positionX;
            this.positionY = positionY;
            this.positionZ = positionZ;
            this.rotation = rotation;
            this.animationId = animationId;
            this.className = className;

            if (initParams.Length != 0)
                this.classParams = LuaUtils.readLuaParams(initParams);

            setPlayerAppearance();
        }

        public void setPlayerAppearance()
        {
            DBAppearance appearance = Database.getAppearance(false, actorId);

            if (appearance == null)
                return;

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
