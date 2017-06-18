using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    class MobController : Controller
    {
        public MobController(Character owner)
        {
            this.owner = owner;
            this.lastUpdate = DateTime.Now;
        }

        public override void Update(DateTime tick)
        {
            // todo: handle aggro/deaggro and other shit here
            ((Mob)this.owner).statusEffects.Update(tick);
        }

        public override bool Engage(Character target)
        {
            // todo: check distance, last swing time, status effects
            this.owner.aiContainer.InternalEngage(target);
            return true;
        }

        private bool TryEngage(Character target)
        {
            // todo:
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

        public override void MobSkill(Character target, uint mobSkillId)
        {
            
        }
    }
}
