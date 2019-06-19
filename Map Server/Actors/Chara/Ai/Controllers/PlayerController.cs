/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using System;
using Meteor.Map.Actors;

namespace Meteor.Map.actors.chara.ai.controllers
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
