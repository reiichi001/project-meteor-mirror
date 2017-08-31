using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class DespawnState : State
    {
        private DateTime endTime;
        public DespawnState(Character owner, Character target, uint despawnTimeSeconds) :
            base(owner, null)
        {
            startTime = Program.Tick;
            endTime = startTime.AddSeconds(despawnTimeSeconds);
        }

        public override bool Update(DateTime tick)
        {
            if (tick >= endTime)
            {
                // todo: send packet to despawn the npc, set it so npc is despawned when requesting spawn packets
                owner.zone.BroadcastPacketAroundActor(owner, RemoveActorPacket.BuildPacket(owner.actorId));
                owner.QueuePositionUpdate(owner.spawnX, owner.spawnY, owner.spawnZ);
                lua.LuaEngine.CallLuaBattleAction(owner, "onDespawn", owner);
                return true;
            }
            return false;
        }
    }
}
