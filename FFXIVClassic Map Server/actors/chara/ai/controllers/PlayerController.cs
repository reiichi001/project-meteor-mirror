using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    class PlayerController : Controller
    {
        public PlayerController(Character owner) :
            base(owner)
        {
            this.lastUpdate = DateTime.Now;
        }

        public override void Update(DateTime tick)
        {
            // todo: handle player stuff on tick
        }

        public override void ChangeTarget(Character target)
        {
            base.ChangeTarget(target);
        }

        public override bool Engage(Character target)
        {
            var canEngage = this.owner.aiContainer.InternalEngage(target);
            if (canEngage)
            {
                // todo: find a better place to put this?
                if (owner.GetState() != SetActorStatePacket.MAIN_STATE_ACTIVE)
                    owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);


                // todo: check speed/is able to move
                // todo: too far, path to player if mob, message if player

                // todo: actual stat based range
                if (Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ) > 10)
                {
                    {
                        // todo: out of range error
                    }
                    ChangeTarget(target);
                    return false;
                }
                // todo: adjust cooldowns with modifiers
            }
            return canEngage;
        }

        public override void Disengage()
        {
            // todo:
            base.Disengage();
            return;
        }

        public override void Cast(Character target, uint spellId)
        {

        }

        public override void Ability(Character target, uint abilityId)
        {

        }

        public override void RangedAttack(Character target)
        {

        }
    }
}
