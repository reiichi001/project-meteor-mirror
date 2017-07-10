using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    class PetController : Controller
    {
        private Character petMaster;

        public PetController(Character owner)
        {
            this.owner = owner;
            this.lastUpdate = Program.Tick;
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

        public Character GetPetMaster()
        {
            return petMaster;
        }

        public void SetPetMaster(Character master)
        {
            petMaster = master;
        }
    }
}
