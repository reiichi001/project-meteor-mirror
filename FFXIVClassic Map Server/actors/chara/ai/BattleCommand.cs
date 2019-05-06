using FFXIVClassic_Map_Server.Actors;
using System;
using System.Collections.Generic;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.actors.chara.ai.utils;
using MoonSharp.Interpreter;

namespace FFXIVClassic_Map_Server.actors.chara.ai
{

    public enum BattleCommandRequirements : ushort
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

    public enum BattleCommandPositionBonus : byte
    {
        None,
        Front = 0x01,
        Rear = 0x02,
        Flank = 0x04
    }

    public enum BattleCommandProcRequirement : byte
    {
        None,
        Miss,
        Evade,
        Parry,
        Block
    }

    public enum BattleCommandValidUser : byte
    {
        All,
        Player,
        Monster
    }

    public enum BattleCommandCastType : ushort
    {
        None,
        Weaponskill = 1,
        Weaponskill2 = 2,
        BlackMagic = 3,
        WhiteMagic = 4,
        SongMagic = 8
    }


    //What type of command it is
    [Flags]
    public enum CommandType : ushort
    {
        //Type of action
        None = 0,
        AutoAttack = 1,
        WeaponSkill = 2,
        Ability =3,
        Spell = 4
    }

    public enum KnockbackType : ushort
    {
        None = 0,
        Level1 = 1,
        Level2 = 2,
        Level3 = 3,
        Level4 = 4,
        Level5 = 5,
        Clockwise1 = 6,
        Clockwise2 = 7,
        CounterClockwise1 = 8,
        CounterClockwise2 = 9,
        DrawIn = 10
    }

    class BattleCommand
    {
        public ushort id;
        public string name;
        public byte job;
        public byte level;
        public BattleCommandRequirements requirements;
        public ValidTarget mainTarget;                      //what the skill has to be used on. ie self for flare, enemy for ring of talons even though both are self-centere aoe
        public ValidTarget validTarget;                     //what type of character the skill can hit
        public TargetFindAOEType aoeType;                   //shape of aoe
        public TargetFindAOETarget aoeTarget;               //where the center of the aoe is (target/self)
        public byte numHits;                                //amount of hits in the skill
        public BattleCommandPositionBonus positionBonus;    //bonus for front/flank/rear
        public BattleCommandProcRequirement procRequirement;//if the skill requires a block/parry/evade before using
        public float range;                                 //maximum distance to target to be able to use this skill
        public float minRange;                              //Minimum distance to target to be able to use this skill

        public uint statusId;                               //id of statuseffect that the skill might inflict
        public uint statusDuration;                         //duration of statuseffect in milliseconds
        public float statusChance;                          //percent chance of status landing, 0-1.0. Usually 1.0 for buffs
        public byte castType;                               //casting animation, 2 for blm, 3 for whm, 8 for brd
        public uint castTimeMs;                             //cast time in milliseconds
        public uint recastTimeMs;                           //recast time in milliseconds
        public uint maxRecastTimeSeconds;                   //maximum recast time in seconds
        public ushort mpCost;
        public ushort tpCost;
        public byte animationType;
        public ushort effectAnimation;
        public ushort modelAnimation;
        public ushort animationDurationSeconds;
        public uint battleAnimation;
        public ushort worldMasterTextId;
        public float aoeRange;                              //Radius for circle and cone aoes, length for box aoes
        public float aoeMinRange;                           //Minimum range of aoe effect for things like Lunar Dynamo or Arrow Helix
        public float aoeConeAngle;                          //Angle of aoe cones
        public float aoeRotateAngle;                        //Amount aoes are rotated about the target position (usually the user's position)
        public float rangeHeight;                           //Total height a skill can be used against target above or below user
        public float rangeWidth;                            //Width of box aoes
        public int[] comboNextCommandId = new int[2];       //next two skills in a combo
        public short comboStep;                             //Where in a combo string this skill is
        public CommandType commandType;
        public ActionProperty actionProperty;
        public ActionType actionType;


        public byte statusTier;                             //tier of status to put on target
        public double statusMagnitude = 0;                  //magnitude of status to put on target
        public ushort basePotency;                          //damage variable
        public float enmityModifier;                        //multiples by damage done to get final enmity
        public float accuracyModifier;                      //modifies accuracy
        public float bonusCritRate;                         //extra crit rate
        public bool isCombo;
        public bool comboEffectAdded = false;               //If the combo effect is added to multiple hiteffects it plays multiple times, so this keeps track of that
        public bool isRanged = false;

        public bool actionCrit;                             //Whether any actions were critical hits, used for Excruciate

        public lua.LuaScript script;                        //cached script

        public TargetFind targetFind;
        public BattleCommandValidUser validUser;

        public BattleCommand(ushort id, string name)
        {
            this.id = id;
            this.name = name;
            this.range = 0;
            this.enmityModifier = 1;
            this.accuracyModifier = 0;
            this.statusTier = 1;
            this.statusChance = 50;
            this.recastTimeMs = (uint) maxRecastTimeSeconds * 1000;
            this.isCombo = false;
        }

        public BattleCommand Clone()
        {
            return (BattleCommand)MemberwiseClone();
        }
        
        public int CallLuaFunction(Character chara, string functionName, params object[] args)
        {
            if (script != null && !script.Globals.Get(functionName).IsNil())
            {
                DynValue res = new DynValue();
                res = script.Call(script.Globals.Get(functionName), args);
                if (res != null)
                    return (int)res.Number;
            }

            return -1;
        }

        public bool IsSpell()
        {
            return mpCost != 0 || castTimeMs != 0;
        }

        public bool IsInstantCast()
        {
            return castTimeMs == 0;
        }

        //Checks whether the skill can be used on the given target
        public bool IsValidMainTarget(Character user, Character target)
        {
            targetFind = new TargetFind(user);

            if (aoeType == TargetFindAOEType.Box)
            {
                targetFind.SetAOEBox(validTarget, aoeTarget, aoeRange, rangeWidth, aoeRotateAngle);
            }
            else
            {
                targetFind.SetAOEType(validTarget, aoeType, aoeTarget, aoeRange, aoeMinRange, rangeHeight, aoeRotateAngle, aoeConeAngle);
            }

            /*
            worldMasterTextId
            32512   cannot be performed on a KO'd target.
            32513	can only be performed on a KO'd target.
            32514	cannot be performed on yourself.
            32515	can only be performed on yourself.
            32516	cannot be performed on a friendly target.
            32517	can only be performed on a friendly target.
            32518	cannot be performed on an enemy.
            32519	can only be performed on an enemy,
            32556   unable to execute [weaponskill]. Conditions for use are not met.
            */

            // cant target dead
            if ((mainTarget & (ValidTarget.Corpse | ValidTarget.CorpseOnly)) == 0 && target.IsDead())
            {
                // cannot be perfomed on
                if (user is Player)
                    ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32512, 0x20, (uint)id);
                return false;
            }

            //level too high
            if (level > user.GetLevel())
            {
                if (user is Player)
                    ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32527, 0x20, (uint)id);
                //return false;
            }

            //Proc requirement
            if (procRequirement != BattleCommandProcRequirement.None && !user.charaWork.battleTemp.timingCommandFlag[(int) procRequirement - 1])
            {
                if (user is Player)
                    ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32556, 0x20, (uint)id);
                return false;
            }

            //costs too much tp
            if (CalculateTpCost(user) > user.GetTP())
            {
                if (user is Player)
                    ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32546, 0x20, (uint)id);
                return false;
            }

            // todo: calculate cost based on modifiers also (probably in BattleUtils)
            if (BattleUtils.CalculateSpellCost(user, target, this) > user.GetMP())
            {
                if (user is Player)
                    ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32545, 0x20, (uint)id);
                return false;
            }

            // todo: check target requirements
            if (requirements != BattleCommandRequirements.None)
            {
                if (false)
                {
                    // Unable to execute [@SHEET(xtx/command,$E8(1),2)]. Conditions for use are not met.
                    if (user is Player)
                        ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32556, 0x20, (uint)id);
                    return false;
                }
            }


            // todo: i dont care to message for each scenario, just the most common ones..
            if ((mainTarget & ValidTarget.CorpseOnly) != 0)
            {
                if (target != null && target.IsAlive())
                {
                    if (user is Player)
                        ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32513, 0x20, (uint)id);
                    return false;
                }
            }

            if ((mainTarget & ValidTarget.Enemy) != 0)
            {
                if (target == user || target != null &&
                    user.allegiance == target.allegiance)
                {
                    if (user is Player)
                        ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32519, 0x20, (uint)id);
                    return false;
                }
            }

            if ((mainTarget & ValidTarget.Ally) != 0)
            {
                if (target == null || target.allegiance != user.allegiance)
                {
                    if (user is Player)
                        ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32517, 0x20, (uint)id);
                    return false;
                }
            }

            if ((mainTarget & ValidTarget.PartyMember) != 0)
            {
                if (target == null || target.currentParty != user.currentParty)
                {
                    if (user is Player)
                        ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32547, 0x20, (uint)id);
                    return false;
                }
            }

            if ((mainTarget & ValidTarget.Player) != 0)
            {
                if (!(target is Player))
                {
                    if (user is Player)
                        ((Player)user).SendGameMessage(Server.GetWorldManager().GetActor(), 32517, 0x20, (uint)id);
                    return false;
                }
            }

            return true;// targetFind.CanTarget(target, true, true, true); //this will be done later
        }

        public ushort CalculateMpCost(Character user)
        {
            // todo: use precalculated costs instead
            var level = user.GetLevel();
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

            //scale the mpcost by level
            cost = (ushort)Math.Ceiling((cost * mpCost * 0.001));

            //if user is player, check if spell is a part of combo
            if (user is Player)
            {
                var player = user as Player;
                if (player.playerWork.comboNextCommandId[0] == id || player.playerWork.comboNextCommandId[1] == id)
                    cost = (ushort)Math.Ceiling(cost * (1 - player.playerWork.comboCostBonusRate));
            }

            return mpCost != 0 ? cost : (ushort)0;
        }

        //Calculate TP cost taking into considerating the combo bonus rate for players
        //Should this set tpCost or should it be called like CalculateMp where it gets calculated each time? 
        //Might cause issues with the delay between starting and finishing a WS
        public ushort CalculateTpCost(Character user)
        {
            ushort tp = tpCost;
            //Calculate tp cost
            if (user is Player)
            {
                var player = user as Player;
                if (player.playerWork.comboNextCommandId[0] == id || player.playerWork.comboNextCommandId[1] == id)
                    tp  = (ushort)Math.Ceiling((float)tpCost * (1 - player.playerWork.comboCostBonusRate));
            }

            return tp;
        }

        public List<Character> GetTargets()
        {
            return targetFind?.GetTargets<Character>();
        }

        public ushort GetCommandType()
        {
            return (ushort) commandType;
        }
    }
}