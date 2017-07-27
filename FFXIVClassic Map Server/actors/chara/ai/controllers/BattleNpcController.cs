using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    class BattleNpcController : Controller
    {
        private DateTime lastActionTime;
        private DateTime lastSpellCastTime;
        private DateTime lastSkillTime;
        private DateTime lastSpecialSkillTime; // todo: i dont think monsters have "2hr" cooldowns like ffxi
        private DateTime deaggroTime;
        private DateTime neutralTime;
        private DateTime waitTime;

        private bool firstSpell = true;
        private DateTime lastRoamScript; // todo: what even is this used as

        public BattleNpcController(Character owner)
        {
            this.owner = owner;
            this.lastUpdate = DateTime.Now;
        }

        public override void Update(DateTime tick)
        {
            var battleNpc = this.owner as BattleNpc;

            if (battleNpc != null)
            {
                // todo: handle aggro/deaggro and other shit here
                if (battleNpc.aiContainer.IsEngaged())
                {
                    DoCombatTick(tick);
                }
                else if (!battleNpc.IsDead())
                {
                    DoRoamTick(tick);
                }
                battleNpc.Update(tick);
            }
        }

        public override bool Engage(Character target)
        {
            // todo: check distance, last swing time, status effects
            var canEngage = this.owner.aiContainer.InternalEngage(target);
            if (canEngage)
            {
                // reset casting
                firstSpell = true;

                // todo: adjust cooldowns with modifiers
            }
            return canEngage;
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

        public override void MonsterSkill(Character target, uint mobSkillId)
        {
            
        }

        private void DoRoamTick(DateTime tick)
        {
            var battleNpc = owner as BattleNpc;
            
            if (battleNpc != null)
            {
                if (battleNpc.hateContainer.GetHateList().Count > 0)
                {
                    Engage(battleNpc.hateContainer.GetMostHatedTarget());
                    return;
                }
                else if (battleNpc.currentLockedTarget != 0)
                {
                    
                }
            }
        }

        private void DoCombatTick(DateTime tick)
        {

        }
    }
}
