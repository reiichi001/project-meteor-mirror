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
    abstract class Controller
    {
        protected Character owner;

        protected DateTime lastCombatTickScript;
        protected DateTime lastUpdate;
        public bool canUpdate = true;
        protected bool autoAttackEnabled = true;
        protected bool castingEnabled = true;
        protected bool weaponSkillEnabled = true;
        protected PathFind pathFind;
        protected TargetFind targetFind;

        public Controller(Character owner)
        {
            this.owner = owner;
        }

        public abstract void Update(DateTime tick);
        public abstract bool Engage(Character target);
        public abstract void Cast(Character target, uint spellId);
        public virtual void WeaponSkill(Character target, uint weaponSkillId) { }
        public virtual void MonsterSkill(Character target, uint mobSkillId) { }
        public virtual void UseItem(Character target, uint slot, uint itemId) { }
        public abstract void Ability(Character target, uint abilityId);
        public abstract void RangedAttack(Character target);
        public virtual void Spawn() { }
        public virtual void Despawn() { }


        public virtual void Disengage()
        {
            owner.aiContainer.InternalDisengage();
        }

        public virtual void ChangeTarget(Character target)
        {
            owner.aiContainer.InternalChangeTarget(target);
        }

        public bool IsAutoAttackEnabled()
        {
            return autoAttackEnabled;
        }

        public void SetAutoAttackEnabled(bool isEnabled)
        {
            autoAttackEnabled = isEnabled;
        }

        public bool IsCastingEnabled()
        {
            return castingEnabled;
        }

        public void SetCastingEnabled(bool isEnabled)
        {
            castingEnabled = isEnabled;
        }

        public bool IsWeaponSkillEnabled()
        {
            return weaponSkillEnabled;
        }

        public void SetWeaponSkillEnabled(bool isEnabled)
        {
            weaponSkillEnabled = isEnabled;
        }
    }
}
