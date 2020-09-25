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
    class PetController : Controller
    {
        private Character petMaster;

        public PetController(Character owner) :
            base(owner)
        {
            this.lastUpdate = Program.Tick;
        }

        public override void Update(DateTime tick)
        {
            // todo: handle pet stuff on tick
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

        public override void Disengage()
        {
            // todo:
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

        public Character GetPetMaster()
        {
            return petMaster;
        }

        public void SetPetMaster(Character master)
        {
            petMaster = master;

            if (master is Player)
                owner.allegiance = CharacterTargetingAllegiance.Player;
            else
                owner.allegiance = CharacterTargetingAllegiance.BattleNpcs;
        }
    }
}
