using FFXIVClassic_Map_Server.Actors;
namespace FFXIVClassic_Map_Server.actors.chara.ai.utils
{
    static class AttackUtils
    {
        public static int CalculateDamage(Character attacker, Character defender)
        {
            int dmg = CalculateBaseDamage(attacker, defender);

            return dmg;
        }

        public static int CalculateBaseDamage(Character attacker, Character defender)
        {
            // todo: actually calculate damage
            return Program.Random.Next(10) * 10;
        }
    }
}