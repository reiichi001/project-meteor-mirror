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
            canInterrupt = false;
            startTime = tick;
            despawnTime = startTime.AddSeconds(timeToFadeOut);
        }

        public override bool Update(DateTime tick)
        {
            // todo: handle raise etc
            if (tick >= despawnTime)
            {
                if (owner.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER)
                {
                    // todo: mark for zoning and remove after main loop
                    owner.Spawn(Program.Tick);
                    //Server.GetWorldManager().DoZoneChange(((Player)owner), 244, null, 0, 15, -160.048f, 0, -165.737f, 0.0f);
                }
                else
                {
                    owner.ChangeState(SetActorStatePacket.MAIN_STATE_PASSIVE);
                    // todo: fadeout animation and crap
                    //owner.zone.DespawnActor(owner);
                }
                return true;
            }
            return false;
        }
    }
}
