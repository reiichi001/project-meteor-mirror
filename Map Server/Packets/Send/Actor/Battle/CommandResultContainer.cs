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

using System.Collections.Generic;

namespace Meteor.Map.packets.send.actor.battle
{
    class CommandResultContainer
    {
        private List<CommandResult> actionsList = new List<CommandResult>();
        
        //EXP messages are always the last mesages in battlea ction packets, so they get appended after all the rest of the actions are done.
        private List<CommandResult> expActionList = new List<CommandResult>(); 

        public CommandResultContainer()
        {

        }

        public void AddAction(uint targetId, ushort worldMasterTextId, uint effectId, ushort amount = 0, byte param = 0, byte hitNum = 0)
        {
            AddAction(new CommandResult(targetId, worldMasterTextId, effectId, amount, param, hitNum));
        }

        //Just to make scripting simpler
        //These have to be split into the normal actions and absorb actions because they use different flags
        //AddMP/HP/TPAction are for actions where the targetID is the person being targeted by command. Like Sanguine Rite would use AddMPAction
        public void AddMPAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.MagicEffectType | HitEffect.MP | HitEffect.Heal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddHPAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.MagicEffectType | HitEffect.Heal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddTPAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.MagicEffectType | HitEffect.TP | HitEffect.Heal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        //These are used for skills where the targetId is the person using a command. For example casting with parsimony would use AddMPAbsorbAction
        public void AddMPAbsorbAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.SelfHealType | HitEffect.SelfHealMP | HitEffect.SelfHeal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddHPAbsorbAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.SelfHealType | HitEffect.SelfHeal | HitEffect.SelfHeal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddTPAbsorbAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.SelfHealType | HitEffect.SelfHealTP | HitEffect.SelfHeal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddHitAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.HitEffectType | HitEffect.Hit);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddAction(CommandResult action)
        {
            if (action != null)
                actionsList.Add(action);
        }
        
        public void AddActions(List<CommandResult> actions)
        {
            actionsList.AddRange(actions);
        }

        public void AddEXPAction(CommandResult action)
        {
            expActionList.Add(action);
        }

        public void AddEXPActions(List<CommandResult> actionList)
        {
            expActionList.AddRange(actionList);
        }

        public void CombineLists()
        {
            actionsList.AddRange(expActionList);
        }

        public List<CommandResult> GetList()
        {
            return actionsList;
        }
    }
}
