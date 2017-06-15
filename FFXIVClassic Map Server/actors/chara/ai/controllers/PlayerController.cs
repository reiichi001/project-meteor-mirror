using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    class PlayerController : Controller
    {
        public PlayerController(Character owner)
        {
            this.owner = owner;
            this.lastUpdate = DateTime.Now;
        }

        public override void Update(DateTime tick)
        {
            // todo: handle player stuff on tick

            ((Player)this.owner).statusEffects.Update(tick);
        }

        public override void ChangeTarget(Character target)
        {
            base.ChangeTarget(target);
        }

        public override bool Engage(Character target)
        {
            // todo: check distance, last swing time, status effects
            return true;
        }

        public override bool Disengage()
        {
            // todo:
            return true;
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
