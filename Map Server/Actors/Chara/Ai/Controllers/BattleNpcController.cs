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

using System;
using System.Collections.Generic;
using Meteor.Common;
using Meteor.Map.Actors;
using Meteor.Map.packets.send.actor;
using Meteor.Map.actors.area;
using Meteor.Map.utils;
using Meteor.Map.actors.chara.ai.state;
using Meteor.Map.actors.chara.npc;

namespace Meteor.Map.actors.chara.ai.controllers
{
    class BattleNpcController : Controller
    {
        protected DateTime lastActionTime;
        protected DateTime lastSpellCastTime;
        protected DateTime lastSkillTime;
        protected DateTime lastSpecialSkillTime; // todo: i dont think monsters have "2hr" cooldowns like ffxi
        protected DateTime deaggroTime;
        protected DateTime neutralTime;
        protected DateTime waitTime;

        private bool firstSpell = true;
        protected DateTime lastRoamUpdate;
        protected DateTime battleStartTime;

        protected new BattleNpc owner;
        public BattleNpcController(BattleNpc owner) :
            base(owner)
        {
            this.owner = owner;
            this.lastUpdate = DateTime.Now;
            this.waitTime = lastUpdate.AddSeconds(5);
        }

        public override void Update(DateTime tick)
        {
            lastUpdate = tick;
            if (!owner.IsDead())
            {
                // todo: handle aggro/deaggro and other shit here
                if (!owner.aiContainer.IsEngaged())
                {
                    TryAggro(tick);
                }

                if (owner.aiContainer.IsEngaged())
                {
                    DoCombatTick(tick);
                }
                //Only move if owner isn't dead and is either too far away from their spawn point or is meant to roam and isn't in combat
                else if (!owner.IsDead() && (owner.isMovingToSpawn || owner.GetMobMod((uint)MobModifier.Roams) > 0))
                {
                    DoRoamTick(tick);
                }
            }
        }

        public bool TryDeaggro()
        {
            if (owner.hateContainer.GetMostHatedTarget() == null || !owner.aiContainer.GetTargetFind().CanTarget(owner.target as Character))
            {
                return true;
            }
            else if (!owner.IsCloseToSpawn())
            {
                return true;
            }
            return false;
        }

        //If the owner isn't moving to spawn, iterate over nearby enemies and 
        //aggro the first one that is within 10 levels and can be detected, then engage
        protected virtual void TryAggro(DateTime tick)
        {
            if (tick >= neutralTime && !owner.isMovingToSpawn)
            {
                if (!owner.neutral && owner.IsAlive())
                {
                    foreach (var chara in owner.zone.GetActorsAroundActor<Character>(owner, 50))
                    {
                        if (chara.allegiance == owner.allegiance)
                           continue;

                        if (owner.aiContainer.pathFind.AtPoint() && owner.detectionType != DetectionType.None)
                        {
                            uint levelDifference = (uint)Math.Abs(owner.GetLevel() - chara.GetLevel());

                            if (levelDifference <= 10 || (owner.detectionType & DetectionType.IgnoreLevelDifference) != 0 && CanAggroTarget(chara))
                            {
                                owner.hateContainer.AddBaseHate(chara);
                                break;
                            }
                        }
                    }
                }
            }

            if (owner.hateContainer.GetHateList().Count > 0)
            {
                Engage(owner.hateContainer.GetMostHatedTarget());
            }
        }

        public override bool Engage(Character target)
        {
            var canEngage = this.owner.aiContainer.InternalEngage(target);
            if (canEngage)
            {
                //owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);

                // reset casting
                firstSpell = true;
                // todo: find a better place to put this?
                if (owner.GetState() != SetActorStatePacket.MAIN_STATE_ACTIVE)
                    owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);

                lastActionTime = DateTime.Now;
                battleStartTime = lastActionTime;
                // todo: adjust cooldowns with modifiers
            }
            return canEngage;
        }

        protected bool TryEngage(Character target)
        {
            // todo:
            return true;
        }

        public override void Disengage()
        {
            var target = owner.target;
            base.Disengage();
            // todo:
            lastActionTime = lastUpdate.AddSeconds(5);
            owner.isMovingToSpawn = true;
            owner.aiContainer.pathFind.SetPathFlags(PathFindFlags.None);
            owner.aiContainer.pathFind.PreparePath(owner.spawnX, owner.spawnY, owner.spawnZ, 1.5f, 10);
            neutralTime = lastActionTime;
            owner.hateContainer.ClearHate();
            lua.LuaEngine.CallLuaBattleFunction(owner, "onDisengage", owner, target, Utils.UnixTimeStampUTC(lastUpdate));
        }

        public override void Cast(Character target, uint spellId)
        {
            // todo:
            if(owner.aiContainer.CanChangeState())
                owner.aiContainer.InternalCast(target, spellId);
        }

        public override void Ability(Character target, uint abilityId)
        {
            // todo:
            if (owner.aiContainer.CanChangeState())
                owner.aiContainer.InternalAbility(target, abilityId);
        }

        public override void RangedAttack(Character target)
        {
            // todo:
        }

        public override void MonsterSkill(Character target, uint mobSkillId)
        {
            // todo:
        }

        protected virtual void DoRoamTick(DateTime tick, List<Character> contentGroupCharas = null)
        {
            if (tick >= waitTime)
            {
                neutralTime = tick.AddSeconds(5);
                if (owner.aiContainer.pathFind.IsFollowingPath())
                {
                    owner.aiContainer.pathFind.FollowPath();
                    lastActionTime = tick.AddSeconds(-5);
                }
                else
                {
                    if (tick >= lastActionTime)
                    {
                        
                    }
                }
                waitTime = tick.AddSeconds(owner.GetMobMod((uint) MobModifier.RoamDelay));
                owner.OnRoam(tick);

                if (CanMoveForward(0.0f) && !owner.aiContainer.pathFind.IsFollowingPath())
                {
                    // will move on next tick
                    owner.aiContainer.pathFind.SetPathFlags(PathFindFlags.None);
                    owner.aiContainer.pathFind.PathInRange(owner.spawnX, owner.spawnY, owner.spawnZ, 1.5f, 50.0f);
                }
                //lua.LuaEngine.CallLuaBattleFunction(owner, "onRoam", owner, contentGroupCharas);
            }

            if (owner.aiContainer.pathFind.IsFollowingPath() && owner.aiContainer.CanFollowPath())
            {
                owner.aiContainer.pathFind.FollowPath();
            }
        }

        protected virtual void DoCombatTick(DateTime tick, List<Character> contentGroupCharas = null)
        {
            HandleHate();
            // todo: magic/attack/ws cooldowns etc
            if (TryDeaggro())
            {
                Disengage();
                return;
            }
            owner.SetMod((uint)Modifier.MovementSpeed, 5);
            if ((tick - lastCombatTickScript).TotalSeconds > 3)
            {
                Move();
                //if (owner.aiContainer.CanChangeState())
                    //owner.aiContainer.WeaponSkill(owner.zone.FindActorInArea<Character>(owner.target.actorId), 27155);
                //lua.LuaEngine.CallLuaBattleFunction(owner, "onCombatTick", owner, owner.target, Utils.UnixTimeStampUTC(tick), contentGroupCharas);
                lastCombatTickScript = tick;
            }
        }

        protected virtual void Move()
        {
            if (!owner.aiContainer.CanFollowPath() || owner.statusEffects.HasStatusEffectsByFlag(StatusEffectFlags.PreventMovement))
            {
                return;
            }

            if (owner.aiContainer.pathFind.IsFollowingScriptedPath())
            {
                owner.aiContainer.pathFind.FollowPath();
                return;
            }

            var vecToTarget = owner.target.GetPosAsVector3() - owner.GetPosAsVector3();
            vecToTarget /= vecToTarget.Length();
            vecToTarget = (Utils.Distance(owner.GetPosAsVector3(), owner.target.GetPosAsVector3()) - owner.GetAttackRange() + 0.2f) * vecToTarget;

            var targetPos = vecToTarget + owner.GetPosAsVector3();
            var distance = Utils.Distance(owner.GetPosAsVector3(), owner.target.GetPosAsVector3());
            if (distance > owner.GetAttackRange() - 0.2f)
            {
                if (CanMoveForward(distance))
                {
                    if (!owner.aiContainer.pathFind.IsFollowingPath() && distance > 3)
                    {
                        // pathfind if too far otherwise jump to target
                        owner.aiContainer.pathFind.SetPathFlags(PathFindFlags.None);
                        owner.aiContainer.pathFind.PreparePath(targetPos, 1.5f, 5);
                    }
                    owner.aiContainer.pathFind.FollowPath();
                    if (!owner.aiContainer.pathFind.IsFollowingPath())
                    {
                        if (owner.target is Player)
                        {
                            foreach (var chara in owner.zone.GetActorsAroundActor<Character>(owner, 1))
                            {
                                if (chara == owner)
                                    continue;

                                float mobDistance = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, chara.positionX, chara.positionY, chara.positionZ);
                                if (mobDistance < 0.50f && (chara.updateFlags & ActorUpdateFlags.Position) == 0)
                                {
                                    owner.aiContainer.pathFind.PathInRange(targetPos, 1.3f, chara.GetAttackRange());
                                    break;
                                }
                            }
                        }
                        FaceTarget();
                    }
                }
            }
            else
            {
                FaceTarget();
            }
            lastRoamUpdate = DateTime.Now;
        }

        protected void FaceTarget()
        {
            // todo: check if stunned etc
            if (owner.statusEffects.HasStatusEffectsByFlag(StatusEffectFlags.PreventTurn) )
            {
            }
            else
            {
                owner.LookAt(owner.target);
            }
        }

        protected bool CanMoveForward(float distance)
        {
            // todo: check spawn leash and stuff
            if (!owner.IsCloseToSpawn())
            {
                return false;
            }
            if (owner.GetSpeed() == 0)
            {
                return false;
            }
            return true;
        }

        public virtual bool CanAggroTarget(Character target)
        {
            if (owner.neutral || owner.detectionType == DetectionType.None || owner.IsDead())
            {
                return false;
            }

            // todo: can mobs aggro mounted targets?
            if (target.IsDead() || target.currentMainState == SetActorStatePacket.MAIN_STATE_MOUNTED)
            {
                return false;
            }

            if (owner.aiContainer.IsSpawned() && !owner.aiContainer.IsEngaged() && CanDetectTarget(target))
            {
                return true;
            }
            return false;
        }

        public virtual bool CanDetectTarget(Character target, bool forceSight = false)
        {
            if (owner.IsDead())
                return false;

            // todo: this should probably be changed to only allow detection at end of path?
            if (owner.aiContainer.pathFind.IsFollowingScriptedPath() || owner.aiContainer.pathFind.IsFollowingPath() && !owner.aiContainer.pathFind.AtPoint())
            {
                return false;
            }

            // todo: handle sight/scent/hp etc
            if (target.IsDead() || target.currentMainState == SetActorStatePacket.MAIN_STATE_MOUNTED)
                return false;

            float verticalDistance = Math.Abs(target.positionY - owner.positionY);
            if (verticalDistance > 8)
                return false;

            var distance = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ);

            bool detectSight = forceSight || (owner.detectionType & DetectionType.Sight) != 0;
            bool hasSneak = false;
            bool hasInvisible = false;
            bool isFacing = owner.IsFacing(target);

            // use the mobmod sight range before defaulting to 20 yalms
            if (detectSight && !hasInvisible && isFacing && distance < owner.GetMobMod((uint)MobModifier.SightRange))
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            // todo: check line of sight and aggroTypes
            if (distance > 20)
            {
                return false;
            }

            // todo: seems ffxiv doesnt even differentiate between sneak/invis?
            {
                hasSneak = target.GetMod(Modifier.Stealth) > 0;
                hasInvisible = hasSneak;
            }



            if ((owner.detectionType & DetectionType.Sound) != 0 && !hasSneak && distance < owner.GetMobMod((uint)MobModifier.SoundRange))
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            if ((owner.detectionType & DetectionType.Magic) != 0 && target.aiContainer.IsCurrentState<MagicState>())
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            if ((owner.detectionType & DetectionType.LowHp) != 0 && target.GetHPP() < 75)
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            return false;
        }

        public virtual bool CanSeePoint(float x, float y, float z)
        {
            return NavmeshUtils.CanSee((Zone)owner.zone, owner.positionX, owner.positionY, owner.positionZ, x, y, z);
        }

        protected virtual void HandleHate()
        {
            ChangeTarget(owner.hateContainer.GetMostHatedTarget());
        }

        public override void ChangeTarget(Character target)
        {
            if (target != owner.target)
            {
                owner.target = target;
                owner.currentLockedTarget = target?.actorId ?? Actor.INVALID_ACTORID;
                owner.currentTarget = target?.actorId ?? Actor.INVALID_ACTORID;

                 foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                    player.QueuePacket(owner.GetHateTypePacket(player));

                base.ChangeTarget(target);
            }
        }
    }
}
