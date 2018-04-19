using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class InactiveState : State
    {
        private DateTime endTime;
        private uint durationMs;
        public InactiveState(Character owner, uint durationMs, bool canChangeState) :
            base(owner, null)
        {
            if (!canChangeState)
                owner.aiContainer.InterruptStates();
            this.durationMs = durationMs;
            endTime = DateTime.Now.AddMilliseconds(durationMs);
        }

        public override bool Update(DateTime tick)
        {
            if (durationMs == 0)
            {
                if (owner.IsDead())
                    return true;

                if (!owner.statusEffects.HasStatusEffectsByFlag(StatusEffectFlags.PreventMovement))
                    return true;
            }

            if (durationMs != 0 && tick > endTime)
            {
                return true;
            }

            return false;
        }
    }
}
