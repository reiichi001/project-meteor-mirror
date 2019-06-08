using System;
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
            owner.Disengage();
            owner.ChangeState(SetActorStatePacket.MAIN_STATE_DEAD);
            owner.statusEffects.RemoveStatusEffectsByFlags((uint)StatusEffectFlags.LoseOnDeath);
            //var deathStatePacket = SetActorStatePacket.BuildPacket(owner.actorId, SetActorStatePacket.MAIN_STATE_DEAD2, owner.currentSubState);
            //owner.zone.BroadcastPacketAroundActor(owner, deathStatePacket);
            canInterrupt = false;
            startTime = tick;
            despawnTime = startTime.AddSeconds(timeToFadeOut);
        }

        public override bool Update(DateTime tick)
        {
            // todo: set a flag on chara for accept raise, play animation and spawn
            if (owner.GetMod((uint)Modifier.Raise) > 0)
            {
                owner.Spawn(tick);
                return true;
            }

            // todo: handle raise etc
            if (tick >= despawnTime)
            {
                // todo: for players, return them to homepoint
                owner.Despawn(tick);
                return true;
            }
            return false;
        }
    }
}
