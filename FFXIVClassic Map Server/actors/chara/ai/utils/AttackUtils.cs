using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
namespace FFXIVClassic_Map_Server.actors.chara.ai.utils
{
    static class AttackUtils
    {
        public static int CalculateDamage(ref Character attacker, ref Character defender)
        {
            int dmg = CalculateBaseDamage(ref attacker, ref defender);

            return dmg;
        }
        public static int CalculateBaseDamage(ref Character attacker, ref Character defender)
        {
            return 0;
        }
    }
}
