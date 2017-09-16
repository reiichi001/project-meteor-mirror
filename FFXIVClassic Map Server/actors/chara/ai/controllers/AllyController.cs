using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.npc;

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

        // server really likes to hang whenever scripts iterate area's actorlist
        protected override void DoCombatTick(DateTime tick, List<Character> contentGroupCharas = null)
        {
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
            base.DoCombatTick(tick, contentGroupCharas);
        }
    }
}
