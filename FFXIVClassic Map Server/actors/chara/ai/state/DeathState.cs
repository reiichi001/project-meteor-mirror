using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class DeathState : State
    {
        DateTime despawnTime;
        public DeathState(Character owner, DateTime tick, uint timeToFadeOut) 
            : base(owner, null)
        {
            owner.ChangeState(SetActorStatePacket.MAIN_STATE_DEAD);
            owner.Disengage();
            canInterrupt = false;
            startTime = tick;
            despawnTime = startTime.AddSeconds(timeToFadeOut);
        }

        public override bool Update(DateTime tick)
        {
            // todo: handle raise etc
            if (tick >= despawnTime)
            {
                if (owner is BattleNpc)
                {
                    owner.Despawn(tick);
                }
                else
                {
                    // todo: queue a warp for the player
                }
                return true;
            }
            return false;
        }
    }
}
