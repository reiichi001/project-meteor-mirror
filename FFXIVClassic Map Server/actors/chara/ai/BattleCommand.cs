using FFXIVClassic_Map_Server.Actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.actors.chara.ai.utils;

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

    class BattleCommand
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
        public int aoeRange;

        public TargetFind targetFind;

        public BattleCommand(ushort id, string name)
        {
            this.id = id;
            this.name = name;
            this.range = 0;
        }

        public BattleCommand Clone()
        {
            return (BattleCommand)MemberwiseClone();
        }

        public bool IsSpell()
        {
            return mpCost != 0 || castTimeSeconds != 0;
        }

        public bool IsInstantCast()
        {
            return castTimeSeconds == 0;
        }

        public bool IsValidTarget(Character user, Character target, ref SubPacket errorPacket)
        {
            // todo: set box length..
            targetFind = new TargetFind(user);
            
            if (aoeType == TargetFindAOEType.Box)
            {
                // todo: read box width from sql
                targetFind.SetAOEBox(validTarget, range, aoeRange);
            }
            else
            {
                targetFind.SetAOEType(validTarget, aoeType, range, aoeRange);
            }


            /*
            worldMasterTextId
            32512 cannot be performed on a KO'd target.
            32513	can only be performed on a KO'd target.
            32514	cannot be performed on yourself.
            32515	can only be performed on yourself.
            32516	cannot be performed on a friendly target.
            32517	can only be performed on a friendly target.
            32518	cannot be performed on an enemy.
            32519	can only be performed on an enemy,
            */

            // cant target dead
            if ((validTarget & (ValidTarget.Corpse | ValidTarget.CorpseOnly)) == 0 && target.IsDead())
            {
                // cannot be perfomed on
                errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32512, 0x20, (uint)id);
                return false;
            }
            if (level > user.charaWork.parameterSave.state_mainSkillLevel)
            {
                errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32527, 0x20, (uint)id);
                return false;
            }

            if (tpCost > user.GetTP())
            {
                errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32546, 0x20, (uint)id);
                return false;
            }

            // todo: calculate cost based on modifiers also (probably in BattleUtils)
            
            if (BattleUtils.CalculateSpellCost(user, target, this) > user.GetMP())
            {
                // todo: error message
                errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32545, 0x20, (uint)id);
                return false;
            }

            // todo: check target requirements
            if (requirements != AbilityRequirements.None)
            {
                if (false)
                {
                    // Unable to execute [@SHEET(xtx/command,$E8(1),2)]. Conditions for use are not met.
                    errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32556, 0x20, (uint)id);
                    return false;
                }
            }

            // todo: i dont care to message for each scenario, just the most common ones..
            if ((validTarget & ValidTarget.CorpseOnly) != 0)
            {
                if (target != null && target.IsAlive())
                {
                    errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32513, 0x20, (uint)id);
                    return false;
                }
            }

            if ((validTarget & ValidTarget.Enemy) != 0)
            {
                if (target == user || target != null &&
                    target.currentSubState != (user.currentSubState == SetActorStatePacket.SUB_STATE_MONSTER ?
                SetActorStatePacket.SUB_STATE_PLAYER : SetActorStatePacket.SUB_STATE_MONSTER))
                {
                    errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32519, 0x20, (uint)id);
                    return false;
                }
            }

            if ((validTarget & (ValidTarget.PartyMember | ValidTarget.Player | ValidTarget.Ally)) != 0)
            {
                if (target == null || target.currentSubState != user.currentSubState )
                {
                    errorPacket = user.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), 32516, 0x20, (uint)id);
                    return false;
                }
            }
            return targetFind.CanTarget(target, true, true);
        }

        public ushort CalculateCost(uint level)
        {
            // todo: use precalculated costs instead
            ushort cost = 0;
            if (level <= 10)
                cost = (ushort)(100 + level * 10);
            else if (level <= 20)
                cost = (ushort)(200 + (level - 10) * 20);
            else if (level <= 30)
                cost = (ushort)(400 + (level - 20) * 40);
            else if (level <= 40)
                cost = (ushort)(800 + (level - 30) * 70);
            else if (level <= 50)
                cost = (ushort)(1500 + (level - 40) * 130);
            else if (level <= 60)
                cost = (ushort)(2800 + (level - 50) * 200);
            else if (level <= 70)
                cost = (ushort)(4800 + (level - 60) * 320);
            else
                cost = (ushort)(8000 + (level - 70) * 500);

            if (mpCost != 0)
                return (ushort)Math.Ceiling((cost * mpCost * 0.001));

            return tpCost;
        }
    }
}
