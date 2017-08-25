﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    class BattleNpcController : Controller
    {
        private DateTime lastActionTime;
        private DateTime lastSpellCastTime;
        private DateTime lastSkillTime;
        private DateTime lastSpecialSkillTime; // todo: i dont think monsters have "2hr" cooldowns like ffxi
        private DateTime deaggroTime;
        private DateTime neutralTime;
        private DateTime waitTime;

        private bool firstSpell = true;
        private DateTime lastRoamScript; // todo: what even is this used as

        private new BattleNpc owner;
        public BattleNpcController(BattleNpc owner) :
            base(owner)
        {
            this.owner = owner;
            this.lastUpdate = DateTime.Now;
            this.waitTime = lastUpdate.AddSeconds(5);
        }

        public override void Update(DateTime tick)
        {
            // todo: handle aggro/deaggro and other shit here
            if (owner.aiContainer.IsEngaged())
            {
                DoCombatTick(tick);
            }
            else if (!owner.IsDead())
            {
                DoRoamTick(tick);
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

        public override bool Engage(Character target)
        {
            // todo: check distance, last swing time, status effects
            var canEngage = this.owner.aiContainer.InternalEngage(target);
            if (canEngage)
            {
                // reset casting
                firstSpell = true;
                // todo: find a better place to put this?
                if (owner.GetState() != SetActorStatePacket.MAIN_STATE_ACTIVE)
                    owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);


                // todo: check speed/is able to move
                // todo: too far, path to player if mob, message if player
                // owner.ResetMoveSpeeds();
                owner.moveState = 2;
                if (owner.currentSubState == SetActorStatePacket.SUB_STATE_MONSTER && owner.moveSpeeds[1] != 0)
                {
                    // todo: actual stat based range
                    if (Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ) > 10)
                    {
                        owner.aiContainer.pathFind.SetPathFlags(PathFindFlags.None);
                        owner.aiContainer.pathFind.PathInRange(target.positionX, target.positionY, target.positionZ, 1.5f, owner.GetAttackRange());
                        ChangeTarget(target);
                        return false;
                    }
                }
                lastActionTime = DateTime.Now;
                // todo: adjust cooldowns with modifiers
            }
            return canEngage;
        }

        private bool TryEngage(Character target)
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
            neutralTime = lastActionTime;
            owner.hateContainer.ClearHate();
            owner.moveState = 1;
            lua.LuaEngine.CallLuaBattleAction(owner, "onDisengage", owner, target);
        }

        public override void Cast(Character target, uint spellId)
        {
            // todo:
        }

        public override void Ability(Character target, uint abilityId)
        {
            // todo:
        }

        public override void RangedAttack(Character target)
        {
            // todo:
        }

        public override void MonsterSkill(Character target, uint mobSkillId)
        {
            // todo:
        }

        private void DoRoamTick(DateTime tick)
        {
            if (owner.hateContainer.GetHateList().Count > 0)
            {
                Engage(owner.hateContainer.GetMostHatedTarget());
                return;
            }
            //else if (owner.currentLockedTarget != 0)
            //{
            //    ChangeTarget(Server.GetWorldManager().GetActorInWorld(owner.currentLockedTarget).GetAsCharacter());
            //}

            if (tick >= waitTime)
            {
                // todo: aggro cooldown
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
                        var battlenpc = owner as BattleNpc;
                        owner.aiContainer.pathFind.PathInRange(battlenpc.spawnX, battlenpc.spawnY, battlenpc.spawnZ, 1.5f, 15.0f);
                    }
                }
                // todo:
                waitTime = tick.AddSeconds(10);
                owner.OnRoam(tick);
            }

            if (owner.aiContainer.pathFind.IsFollowingPath())
            {
                owner.aiContainer.pathFind.FollowPath();
            }
        }

        private void DoCombatTick(DateTime tick)
        {
            HandleHate();

            // todo: magic/attack/ws cooldowns etc
            if (TryDeaggro())
            {
                Disengage();
                return;
            }

            Move();
        }

        private void Move()
        {
            if (!owner.aiContainer.CanFollowPath())
            {
                return;
            }

            if (owner.aiContainer.pathFind.IsFollowingScriptedPath())
            {
                owner.aiContainer.pathFind.FollowPath();
                return;
            }

            var targetPos = new Vector3(owner.target.positionX, owner.target.positionY, owner.target.positionZ);
            var distance = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, targetPos.X, targetPos.Y, targetPos.Z);

            if (distance > owner.GetAttackRange() - 0.2f || owner.aiContainer.CanFollowPath())
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
                        if (owner.target.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER)
                        {
                            foreach (var battlenpc in owner.zone.GetActorsAroundActor<BattleNpc>(owner, 1))
                            {
                                if (battlenpc == owner)
                                    continue;
                                float mobDistance = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, battlenpc.positionX, battlenpc.positionY, battlenpc.positionZ);
                                if (mobDistance < 0.25f && (battlenpc.updateFlags & ActorUpdateFlags.Position) == 0)
                                    battlenpc.aiContainer.pathFind.PathInRange(targetPos, 1.3f, 1.8f);
                            }
                        }
                    }
                }
            }
            else
            {
                FaceTarget();
            }
        }

        private void FaceTarget()
        {
            // todo: check if stunned etc
            if (owner.statusEffects.HasStatusEffectsByFlag(StatusEffectFlags.PreventAction))
            {
            }
            else
            {
                owner.LookAt(owner.target);
            }
        }

        private bool CanMoveForward(float distance)
        {
            // todo: check spawn leash and stuff
            if (!owner.IsCloseToSpawn())
            {
                return false;
            }
            return true;
        }

        public bool CanAggroTarget(Character target)
        {
            if (owner.neutral || owner.aggroType == AggroType.None || owner.IsDead())
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

        public bool CanDetectTarget(Character target, bool forceSight = false)
        {
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

            bool detectSight = forceSight || (owner.aggroType & AggroType.Sight) != 0;
            bool hasSneak = false;
            bool hasInvisible = false;
            bool isFacing = owner.IsFacing(target);

            // todo: check line of sight and aggroTypes
            if (distance > 20)
            {
                return false;
            }

            // todo: seems ffxiv doesnt even differentiate between sneak/invis?
            {
                hasSneak = target.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.Stealth);
                hasInvisible = hasSneak;
            }

            if (detectSight && !hasInvisible && isFacing)
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            if ((owner.aggroType & AggroType.LowHp) != 0 && target.GetHPP() < 75)
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            return false;
        }

        public bool CanSeePoint(float x, float y, float z)
        {
            return NavmeshUtils.CanSee((Zone)owner.zone, owner.positionX, owner.positionY, owner.positionZ, x, y, z);
        }

        private void HandleHate()
        {
            ChangeTarget(owner.hateContainer.GetMostHatedTarget());
        }
    }
}
