using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.Actors;

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

        public void addProperty(string property)
        {
            if (!currentActorPropertyPacket.addProperty(forActor, property))
            {
                currentActorPropertyPacket.setIsMore(true);
                currentActorPropertyPacket.addTarget();
                subPackets.Add(currentActorPropertyPacket.buildPacket(playerActorId, forActor.actorId));
                currentActorPropertyPacket = new SetActorPropetyPacket(currentTarget);
            }
        }

        public void newTarget(string target)
        {
            currentActorPropertyPacket.addTarget();
            currentTarget = target;
            currentActorPropertyPacket.setTarget(target);            
        }

        public List<SubPacket> done()
        {
            currentActorPropertyPacket.addTarget();
            currentActorPropertyPacket.setIsMore(false);
            subPackets.Add(currentActorPropertyPacket.buildPacket(playerActorId, forActor.actorId));
            return subPackets;
        }

    }
}
