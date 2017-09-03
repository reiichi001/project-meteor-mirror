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
        private DateTime respawnTime;
        public DespawnState(Character owner, uint respawnTimeSeconds) :
            base(owner, null)
        {
            startTime = Program.Tick;
            respawnTime = startTime.AddSeconds(respawnTimeSeconds);
            owner.ChangeState(SetActorStatePacket.MAIN_STATE_DEAD2);
            owner.OnDespawn();
        }

        public override bool Update(DateTime tick)
        {
            if (tick >= respawnTime)
            {
                owner.Spawn(tick);
                return true;
            }
            return false;
        }
    }
}
