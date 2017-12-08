using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
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
