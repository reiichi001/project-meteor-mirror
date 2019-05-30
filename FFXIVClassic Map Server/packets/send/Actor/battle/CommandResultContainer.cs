﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.battle
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
