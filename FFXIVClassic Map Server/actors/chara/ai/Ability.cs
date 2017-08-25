using FFXIVClassic_Map_Server.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.actors.chara.player;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{

    public enum AbilityRequirements : ushort
    {
        None,
        DiscipleOfWar = 0x01,
        DiscipeOfMagic = 0x02,
        HandToHand = 0x04,
        Sword = 0x08,
        Shield = 0x10,
        Axe = 0x20,
        Archery = 0x40,
        Polearm = 0x80,
        Thaumaturgy = 0x100,
        Conjury = 0x200
    }

    public enum AbilityPositionBonus : byte
    {
        None,
        Front = 0x01,
        Rear = 0x02,
        Flank = 0x04
    }

    public enum AbilityProcRequirement : byte
    {
        None,
        Evade = 0x01,
        Block = 0x02,
        Parry = 0x04,
        Miss = 0x08
    }

    class Ability
    {
        public ushort id;
        public string name;
        public byte job;
        public byte level;
        public AbilityRequirements requirements;
        public ValidTarget validTarget;
        public TargetFindAOEType aoeType;
        public byte numHits;
        public AbilityPositionBonus positionBonus;
        public AbilityProcRequirement procRequirement;
        public int range;
        public uint debuffDurationSeconds;
        public uint buffDurationSeconds;
        public byte castType;
        public uint castTimeSeconds;
        public uint recastTimeSeconds;
        public ushort mpCost;
        public ushort tpCost;
        public byte animationType;
        public ushort effectAnimation;
        public ushort modelAnimation;
        public ushort animationDurationSeconds;

        public uint battleAnimation;
        public ushort worldMasterTextId;
        public uint param;

        public TargetFind targetFind;

        public Ability(ushort id, string name)
        {
            this.id = id;
            this.name = name;
            this.range = 0;
        }

        public Ability Clone()
        {
            return (Ability)MemberwiseClone();
        }

        public bool IsSpell()
        {
            return mpCost != 0 || castTimeSeconds != 0;
        }

        public bool IsInstantCast()
        {
            return castTimeSeconds == 0;
        }

        public bool IsValidTarget(Character user, Character target)
        {
            // todo: set box length..
            targetFind = new TargetFind(user);
            
            if (aoeType == TargetFindAOEType.Box)
            {
                // todo: read box width from sql
                targetFind.SetAOEBox(validTarget, range, 3);
            }
            else
            {
                targetFind.SetAOEType(validTarget, aoeType, range, 40);
            }
            return targetFind.CanTarget(target, true, true);
        }
    }
}
