using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.battle
{
    class BattleActionContainer
    {
        private List<BattleAction> actionsList = new List<BattleAction>();
        
        //EXP messages are always the last mesages in battlea ction packets, so they get appended after all the rest of the actions are done.
        private List<BattleAction> expActionList = new List<BattleAction>(); 

        public BattleActionContainer()
        {

        }

        public void AddAction(uint targetId, ushort worldMasterTextId, uint effectId, ushort amount = 0, byte param = 0, byte hitNum = 0)
        {
            AddAction(new BattleAction(targetId, worldMasterTextId, effectId, amount, param, hitNum));
        }

        //Just to make scripting simpler
        public void AddMPAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint) (HitEffect.MagicEffectType | HitEffect.MP | HitEffect.Heal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddHPAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint)(HitEffect.MagicEffectType | HitEffect.Heal);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddTPAction(uint targetId, ushort worldMasterTextId, ushort amount)
        {
            uint effectId = (uint)(HitEffect.MagicEffectType | HitEffect.TP);
            AddAction(targetId, worldMasterTextId, effectId, amount);
        }

        public void AddAction(BattleAction action)
        {
            if (action != null)
                actionsList.Add(action);
        }
        
        public void AddActions(List<BattleAction> actions)
        {
            actionsList.AddRange(actions);
        }

        public void AddEXPAction(BattleAction action)
        {
            expActionList.Add(action);
        }

        public void AddEXPActions(List<BattleAction> actionList)
        {
            expActionList.AddRange(actionList);
        }

        public void CombineLists()
        {
            actionsList.AddRange(expActionList);
        }

        public List<BattleAction> GetList()
        {
            return actionsList;
        }
    }
}
