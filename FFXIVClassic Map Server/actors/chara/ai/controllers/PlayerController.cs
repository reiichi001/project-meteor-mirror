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
        private new Player owner;
        public PlayerController(Player owner) :
            base(owner)
        {
            this.owner = owner;
            this.lastUpdate = DateTime.Now;
        }

        public override void Update(DateTime tick)
        {
            /*
            if (owner.newMainState != owner.currentMainState)
            {
                if (owner.newMainState == SetActorStatePacket.MAIN_STATE_ACTIVE)
                {
                    owner.Engage();
                }
                else
                {
                    owner.Disengage();
                }
                owner.currentMainState = (ushort)owner.newMainState;
            }*/
        }

        public override void ChangeTarget(Character target)
        {
            owner.target = target;
            base.ChangeTarget(target);
        }

        public override bool Engage(Character target)
        {
            var canEngage = this.owner.aiContainer.InternalEngage(target);
            if (canEngage)
            {
                if (owner.statusEffects.HasStatusEffect(StatusEffectId.Sleep))
                {
                    // That command cannot be performed.
                    owner.SendGameMessage(Server.GetWorldManager().GetActor(), 32553, 0x20);
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
            owner.aiContainer.InternalCast(target, spellId);
        }

        public override void WeaponSkill(Character target, uint weaponSkillId)
        {
            owner.aiContainer.InternalWeaponSkill(target, weaponSkillId);
        }

        public override void Ability(Character target, uint abilityId)
        {
            owner.aiContainer.InternalAbility(target, abilityId);
        }

        public override void RangedAttack(Character target)
        {

        }

        public override void UseItem(Character target, uint slot, uint itemId)
        {
            owner.aiContainer.InternalUseItem(target, slot, itemId);
        }
    }
}
