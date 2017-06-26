
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.Actors.Chara;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Map_Server.Actors
{
    class Character:Actor
    {
        public const int SIZE = 0;
        public const int COLORINFO = 1;
        public const int FACEINFO = 2;
        public const int HIGHLIGHT_HAIR = 3;
        public const int VOICE = 4;
        public const int MAINHAND = 5;
        public const int OFFHAND = 6;
        public const int SPMAINHAND = 7;
        public const int SPOFFHAND = 8;
        public const int THROWING = 9;
        public const int PACK = 10;
        public const int POUCH = 11;
        public const int HEADGEAR = 12;
        public const int BODYGEAR = 13;
        public const int LEGSGEAR = 14;
        public const int HANDSGEAR = 15;
        public const int FEETGEAR = 16;
        public const int WAISTGEAR = 17;
        public const int NECKGEAR = 18;
        public const int L_EAR = 19;
        public const int R_EAR = 20;
        public const int R_WRIST = 21;
        public const int L_WRIST = 22;
        public const int R_RINGFINGER = 23;
        public const int L_RINGFINGER = 24;
        public const int R_INDEXFINGER = 25;
        public const int L_INDEXFINGER = 26;
        public const int UNKNOWN = 27;

        public bool isStatic = false;

        public uint modelId;
        public uint[] appearanceIds = new uint[28];

        public uint animationId = 0;

        public uint currentTarget = 0xC0000000;
        public uint currentLockedTarget = 0xC0000000;

        public uint currentActorIcon = 0;

        public Work work = new Work();
        public CharaWork charaWork = new CharaWork();

        public Group currentParty = null;
        public ContentGroup currentContentGroup = null;

        public Character(uint actorID) : base(actorID)
        {            
            //Init timer array to "notimer"
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
                charaWork.statusShownTime[i] = 0xFFFFFFFF;
        }

        public SubPacket CreateAppearancePacket(uint playerActorId)
        {
            SetActorAppearancePacket setappearance = new SetActorAppearancePacket(modelId, appearanceIds);
            return setappearance.BuildPacket(actorId, playerActorId);
        }

        public SubPacket CreateInitStatusPacket(uint playerActorId)
        {
            return (SetActorStatusAllPacket.BuildPacket(actorId, playerActorId, charaWork.status));                      
        }

        public SubPacket CreateSetActorIconPacket(uint playerActorId)
        {
            return SetActorIconPacket.BuildPacket(actorId, playerActorId, currentActorIcon);
        }

        public SubPacket CreateIdleAnimationPacket(uint playerActorId)
        {
            return SetActorSubStatPacket.BuildPacket(actorId, playerActorId, 0, 0, 0, 0, 0, 0, animationId);
        }

        public void SetQuestGraphic(Player player, int graphicNum)
        {
            player.QueuePacket(SetActorQuestGraphicPacket.BuildPacket(player.actorId, actorId, graphicNum));
        }

        public void SetCurrentContentGroup(ContentGroup group)
        {
            if (group != null)
                charaWork.currentContentGroup = group.GetTypeId();
            else
                charaWork.currentContentGroup = 0;

            currentContentGroup = group;

            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("charaWork/currentContentGroup", this, actorId);
            propPacketUtil.AddProperty("charaWork.currentContentGroup");            
            zone.BroadcastPacketsAroundActor(this, propPacketUtil.Done());

        }     
   
        public void PlayAnimation(uint animId, bool onlySelf = false)
        {            
            if (onlySelf)
            {
                if (this is Player)
                    ((Player)this).QueuePacket(PlayAnimationOnActorPacket.BuildPacket(actorId, actorId, animId));
            }
            else
                zone.BroadcastPacketAroundActor(this, PlayAnimationOnActorPacket.BuildPacket(actorId, actorId, animId));
        }

    }

}
