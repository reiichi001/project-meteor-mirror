using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.utils
{
    static class BattleUtils
    {
        public static void DamageTarget(Character attacker, Character defender, int damage)
        {
            // todo: other stuff too
            if (defender is BattleNpc)
            {
                if (!((BattleNpc)defender).hateContainer.HasHateForTarget(attacker))
                {
                    ((BattleNpc)defender).hateContainer.AddBaseHate(attacker);
                }
                ((BattleNpc)defender).hateContainer.UpdateHate(attacker, damage);
            }
        }
    }
}
