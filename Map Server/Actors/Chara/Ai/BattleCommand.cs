/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using Meteor.Map.Actors;
using System;
using System.Collections.Generic;
using Meteor.Map.packets.send.actor.battle;
using Meteor.Map.actors.chara.ai.utils;
using MoonSharp.Interpreter;

namespace Meteor.Map.actors.chara.ai
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
        public short mpCost;                                //short in case these casts can have negative cost
        public short tpCost;                                //short because there are certain cases where we want weaponskills to have negative costs (such as Feint)
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

        //Checks whether the skill can be used on the given targets, uses error to return specific text ids for errors
        public bool IsValidMainTarget(Character user, Character target, CommandResult error = null)
        {
            targetFind = new TargetFind(user, target);

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
            32511   Target does not exist
            32512   cannot be performed on a KO'd target.
            32513	can only be performed on a KO'd target.
            32514	cannot be performed on yourself.
            32515	can only be performed on yourself.
            32516	cannot be performed on a friendly target.
            32517	can only be performed on a friendly target.
            32518	cannot be performed on an enemy.
            32519	can only be performed on an enemy.
            32547   That command cannot be performed on the current target.
            32548   That command cannot be performed on a party member
            */
            if (target == null)
            {
                error?.SetTextId(32511);
                return false;
            }

            //This skill can't be used on a corpse and target is dead
            if ((mainTarget & ValidTarget.Corpse) == 0 && target.IsDead())
            {
                error?.SetTextId(32512);
                return false;
            }

            //This skill must be used on a corpse and target is alive
            if ((mainTarget & ValidTarget.CorpseOnly) != 0 && target.IsAlive())
            {
                error?.SetTextId(32513);
                return false;
            }

            //This skill can't be used on self and target is self
            if ((mainTarget & ValidTarget.Self) == 0 && target == user)
            {
                error?.SetTextId(32514);
                return false;
            }

            //This skill must be used on self and target isn't self
            if ((mainTarget & ValidTarget.SelfOnly) != 0 && target != user)
            {
                error?.SetTextId(32515);
                return false;
            }

            //This skill can't be used on an ally and target is an ally
            if ((mainTarget & ValidTarget.Ally) == 0 && target.allegiance == user.allegiance)
            {
                error?.SetTextId(32516);
                return false;
            }

            //This skill must be used on an ally and target is not an ally
            if ((mainTarget & ValidTarget.AllyOnly) != 0 && target.allegiance != user.allegiance)
            {
                error?.SetTextId(32517);
                return false;
            }

            //This skill can't be used on an enemu and target is an enemy
            if ((mainTarget & ValidTarget.Enemy) == 0 && target.allegiance != user.allegiance)
            {
                error?.SetTextId(32518);
                return false;
            }

            //This skill must be used on an enemy and target is an ally
            if ((mainTarget & ValidTarget.EnemyOnly) != 0 && target.allegiance == user.allegiance)
            {
                error?.SetTextId(32519);
                return false;
            }

            //This skill can't be used on party members and target is a party member
            if ((mainTarget & ValidTarget.Party) == 0 && target.currentParty == user.currentParty)
            {
                error?.SetTextId(32548);
                return false;
            }

            //This skill must be used on party members and target is not a party member
            if ((mainTarget & ValidTarget.PartyOnly) != 0 && target.currentParty != user.currentParty)
            {
                error?.SetTextId(32547);
                return false;
            }

            //This skill can't be used on NPCs and target is an npc
            if ((mainTarget & ValidTarget.NPC) == 0 && target.isStatic)
            {
                error?.SetTextId(32547);
                return false;
            }

            //This skill must be used on NPCs and target is not an npc
            if ((mainTarget & ValidTarget.NPCOnly) != 0 && !target.isStatic)
            {
                error?.SetTextId(32547);
                return false;
            }

            // todo: why is player always zoning?
            // cant target if zoning
            if (target is Player && ((Player)target).playerSession.isUpdatesLocked)
            {
                user.aiContainer.ChangeTarget(null);
                return false;
            }

            if (target.zone != user.zone)
                return false;

            return true;
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
        public short CalculateTpCost(Character user)
        {
            short tp = tpCost;
            //Calculate tp cost
            if (user is Player)
            {
                var player = user as Player;
                if (player.playerWork.comboNextCommandId[0] == id || player.playerWork.comboNextCommandId[1] == id)
                    tp  = (short)Math.Ceiling((float)tpCost * (1 - player.playerWork.comboCostBonusRate));
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

        public ushort GetActionType()
        {
            return (ushort) actionType;
        }
    }
}
