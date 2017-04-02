
using System.Collections.Generic;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.utils
{
    class ActorPropertyPacketUtil
    {
        private Actor forActor;
        private uint playerActorId;
        private List<SubPacket> subPackets = new List<SubPacket>();
        private SetActorPropetyPacket currentActorPropertyPacket;
        private string currentTarget;

        public ActorPropertyPacketUtil(string firstTarget, Actor forActor, uint playerActorId)
        {
            currentActorPropertyPacket = new SetActorPropetyPacket(firstTarget);
            this.forActor = forActor;
            this.playerActorId = playerActorId;
            this.currentTarget = firstTarget;
        }

        public void AddProperty(string property)
        {
            if (!currentActorPropertyPacket.AddProperty(forActor, property))
            {
                currentActorPropertyPacket.SetIsMore(true);
                currentActorPropertyPacket.AddTarget();
                subPackets.Add(currentActorPropertyPacket.BuildPacket(playerActorId, forActor.actorId));
                currentActorPropertyPacket = new SetActorPropetyPacket(currentTarget);
                currentActorPropertyPacket.AddProperty(forActor, property);
            }
        }

        public void NewTarget(string target)
        {
            currentActorPropertyPacket.AddTarget();
            currentTarget = target;
            currentActorPropertyPacket.SetTarget(target);            
        }

        public List<SubPacket> Done()
        {
            currentActorPropertyPacket.AddTarget();
            currentActorPropertyPacket.SetIsMore(false);
            subPackets.Add(currentActorPropertyPacket.BuildPacket(playerActorId, forActor.actorId));
            return subPackets;
        }

    }
}
