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
using Meteor.Map.Actors;
using Meteor.Map.actors.chara.ai.state;
using Meteor.Map.actors.chara.ai.controllers;
using Meteor.Map.packets.send.actor;

// port of ai code in dsp by kjLotus (https://github.com/DarkstarProject/darkstar/blob/master/src/map/ai)
namespace Meteor.Map.actors.chara.ai
{
    class AIContainer
    {
        private Character owner;
        private Controller controller;
        private Stack<State> states;
        private DateTime latestUpdate;
        private DateTime prevUpdate;
        public readonly PathFind pathFind;
        private TargetFind targetFind;
        private ActionQueue actionQueue;
        private DateTime lastActionTime;

        public AIContainer(Character actor, Controller controller, PathFind pathFind, TargetFind targetFind)
        {
            this.owner = actor;
            this.states = new Stack<State>();
            this.controller = controller;
            this.pathFind = pathFind;
            this.targetFind = targetFind;
            latestUpdate = DateTime.Now;
            prevUpdate = latestUpdate;
            actionQueue = new ActionQueue(owner);
        }

        public void UpdateLastActionTime(uint delay = 0)
        {
            lastActionTime = DateTime.Now.AddSeconds(delay);
        }

        public DateTime GetLastActionTime()
        {
            return lastActionTime;
        }

        public void Update(DateTime tick)
        {
            prevUpdate = latestUpdate;
            latestUpdate = tick;

            // todo: trigger listeners

            if (controller == null && pathFind != null)
            {
                pathFind.FollowPath();
            }

            // todo: action queues
            if (controller != null && controller.canUpdate)
                controller.Update(tick);

            State top;

            while (states.Count > 0 && (top = states.Peek()).Update(tick))
            {
                if (top == GetCurrentState())
                {
                    states.Pop().Cleanup();
                }
            }
            owner.PostUpdate(tick);
        }

        public void CheckCompletedStates()
        {
            while (states.Count > 0 && states.Peek().IsCompleted())
            {
                states.Peek().Cleanup();
                states.Pop();
            }
        }

        public void InterruptStates()
        {
            while (states.Count > 0 && states.Peek().CanInterrupt())
            {
                states.Peek().SetInterrupted(true);
                states.Peek().Cleanup();
                states.Pop();
            }
        }

        public void InternalUseItem(Character target, uint slot, uint itemId)
        {
            // todo: can allies use items?
            if (owner is Player)
            {
                if (CanChangeState())
                {
                    ChangeState(new ItemState((Player)owner, target, (ushort)slot, itemId));
                }
                else
                {
                    // You cannot use that item now.
                    ((Player)owner).SendGameMessage(Server.GetWorldManager().GetActor(), 32544, 0x20, itemId);
                }
            }
        }

        public void ClearStates()
        {
            while (states.Count > 0)
            {
                states.Peek().Cleanup();
                states.Pop();
            }
        }

        public void ChangeController(Controller controller)
        {
            this.controller = controller;
        }

        public T GetController<T>() where T : Controller
        {
            return controller as T;
        }

        public TargetFind GetTargetFind()
        {
            return targetFind;
        }

        public bool CanFollowPath()
        {
            return pathFind != null && (GetCurrentState() == null || GetCurrentState().CanChangeState());
        }

        public bool CanChangeState()
        {
            return GetCurrentState() == null || states.Peek().CanChangeState();
        }

        public void ChangeTarget(Character target)
        {
            if (controller != null)
            {
                controller.ChangeTarget(target);
            }
        }

        public void ChangeState(State state)
        {
            if (CanChangeState())
            {
                if (states.Count <= 10)
                {
                    CheckCompletedStates();
                    states.Push(state);
                }
                else
                {
                    throw new Exception("shit");
                }
            }
        }

        public void ForceChangeState(State state)
        {
            if (states.Count <= 10)
            {
                CheckCompletedStates();
                states.Push(state);
            }
            else
            {
                throw new Exception("force shit");
            }
        }

        public bool IsCurrentState<T>() where T : State
        {
            return GetCurrentState() is T;
        }

        public State GetCurrentState()
        {
            return states.Count > 0 ? states.Peek() : null;
        }

        public DateTime GetLatestUpdate()
        {
            return latestUpdate;
        }

        public void Reset()
        {
            // todo: reset cooldowns and stuff here too?
            targetFind?.Reset();
            pathFind?.Clear();
            ClearStates();
            InternalDisengage();
        }

        public bool IsSpawned()
        {
            return !IsDead();
        }

        public bool IsEngaged()
        {
            return owner.currentMainState == SetActorStatePacket.MAIN_STATE_ACTIVE;
        }

        public bool IsDead()
        {
            return owner.currentMainState == SetActorStatePacket.MAIN_STATE_DEAD ||
                owner.currentMainState == SetActorStatePacket.MAIN_STATE_DEAD2;
        }

        public bool IsRoaming()
        {
            // todo: check mounted?
            return owner.currentMainState == SetActorStatePacket.MAIN_STATE_PASSIVE;
        }

        public void Engage(Character target)
        {
            if (controller != null)
                controller.Engage(target);
            else
                InternalEngage(target);
        }

        public void Disengage()
        {
            if (controller != null)
                controller.Disengage();
            else
                InternalDisengage();
        }

        public void Ability(Character target, uint abilityId)
        {
            if (controller != null)
                controller.Ability(target, abilityId);
            else
                InternalAbility(target, abilityId);
        }

        public void Cast(Character target, uint spellId)
        {
            if (controller != null)
                controller.Cast(target, spellId);
            else
                InternalCast(target, spellId);
        }

        public void WeaponSkill(Character target, uint weaponSkillId)
        {
            if (controller != null)
                controller.WeaponSkill(target, weaponSkillId);
            else
                InternalWeaponSkill(target, weaponSkillId);
        }

        public void MobSkill(Character target, uint mobSkillId)
        {
            if (controller != null)
                controller.MonsterSkill(target, mobSkillId);
            else
                InternalMobSkill(target, mobSkillId);
        }

        public void UseItem(Character target, uint slot, uint itemId)
        {
            if (controller != null)
                controller.UseItem(target, slot, itemId);
        }

        public void InternalChangeTarget(Character target)
        {
            // targets are changed in the controller
            if (IsEngaged() || target == null)
            {

            }
            else
            {
                Engage(target);
            }
        }

        public bool InternalEngage(Character target)
        {
            if (IsEngaged())
            {
                if (this.owner.target != target)
                {
                    ChangeTarget(target);
                    return true;
                }
                return false;
            }

            if (CanChangeState() || (GetCurrentState() != null && GetCurrentState().IsCompleted()))
            {
                ForceChangeState(new AttackState(owner, target));
                return true;
            }
            return false;
        }

        public void InternalDisengage()
        {
            pathFind?.Clear();
            GetTargetFind()?.Reset();

            owner.updateFlags |= ActorUpdateFlags.HpTpMp;
            ChangeTarget(null);

            if (owner.currentMainState == SetActorStatePacket.MAIN_STATE_ACTIVE)
                owner.ChangeState(SetActorStatePacket.MAIN_STATE_PASSIVE);

            ClearStates();
        }

        public void InternalAbility(Character target, uint abilityId)
        {
            if (CanChangeState())
            {
                ChangeState(new AbilityState(owner, target, (ushort)abilityId));
            }
        }

        public void InternalCast(Character target, uint spellId)
        {
            if (CanChangeState())
            {
                ChangeState(new MagicState(owner, target, (ushort)spellId));
            }
        }

        public void InternalWeaponSkill(Character target, uint weaponSkillId)
        {
            if (CanChangeState())
            {
                ChangeState(new WeaponSkillState(owner, target, (ushort)weaponSkillId));
            }
        }

        public void InternalMobSkill(Character target, uint mobSkillId)
        {
            if (CanChangeState())
            {

            }
        }

        public void InternalDie(DateTime tick, uint fadeoutTimerSeconds)
        {
            pathFind?.Clear();
            ClearStates();
            ForceChangeState(new DeathState(owner, tick, fadeoutTimerSeconds));
        }

        public void InternalDespawn(DateTime tick, uint respawnTimerSeconds)
        {
            ClearStates();
            Disengage();
            ForceChangeState(new DespawnState(owner, respawnTimerSeconds));
        }

        public void InternalRaise(Character target)
        {
            // todo: place at target
            // ForceChangeState(new RaiseState(target));
        }
    }
}
