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
using System.Collections.Generic;
using Meteor.Map.Actors;
using Meteor.Map.actors.chara.npc;

namespace Meteor.Map.actors.chara.ai.controllers
{
    // todo: this is probably not needed, can do everything in their script
    class AllyController : BattleNpcController
    {
        protected new Ally owner;
        public AllyController(Ally owner) :
            base(owner)
        {
            this.owner = owner;
        }
        
        protected List<Character> GetContentGroupCharas()
        {
            List<Character> contentGroupCharas = null;

            if (owner.currentContentGroup != null)
            {
                contentGroupCharas = new List<Character>(owner.currentContentGroup.GetMemberCount());
                foreach (var charaId in owner.currentContentGroup.GetMembers())
                {
                    var chara = owner.zone.FindActorInArea<Character>(charaId);

                    if (chara != null)
                        contentGroupCharas.Add(chara);
                }
            }

            return contentGroupCharas;
        }

        //Iterate over players in the group and if they are fighting, assist them
        protected override void TryAggro(DateTime tick)
        {
            //lua.LuaEngine.CallLuaBattleFunction(owner, "tryAggro", owner, GetContentGroupCharas());

            foreach(Character chara in GetContentGroupCharas())
            {
                if(chara.IsPlayer())
                {
                    if(owner.aiContainer.GetTargetFind().CanTarget((Character) chara.target) && chara.target is BattleNpc && ((BattleNpc)chara.target).hateContainer.HasHateForTarget(chara))
                    {
                        owner.Engage(chara.target.actorId);
                        owner.hateContainer.AddBaseHate((Character) chara.target);
                        break;
                    }
                }
            }
            //base.TryAggro(tick);
        }

        // server really likes to hang whenever scripts iterate area's actorlist
        protected override void DoCombatTick(DateTime tick, List<Character> contentGroupCharas = null)
        {
            if (contentGroupCharas == null)
            {
                contentGroupCharas = GetContentGroupCharas();
            }

            base.DoCombatTick(tick, contentGroupCharas);
        }

        protected override void DoRoamTick(DateTime tick, List<Character> contentGroupCharas = null)
        {
            if (contentGroupCharas == null)
            {
                contentGroupCharas = GetContentGroupCharas();
            }
            base.DoRoamTick(tick, contentGroupCharas);
        }
    }
}
