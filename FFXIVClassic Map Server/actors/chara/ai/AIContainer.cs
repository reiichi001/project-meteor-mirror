using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.ai.state;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.packets.send.actor;

// port of ai code in dsp by kjLotus (https://github.com/DarkstarProject/darkstar/blob/master/src/map/ai)
namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    // todo: actually implement stuff
    // todo: use spell/ability/ws/mobskill objects instead of looking up ids
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

        public Controller GetController()
        {
            return controller;
        }

        public TargetFind GetTargetFind()
        {
            return targetFind;
        }

        public bool CanFollowPath()
        {
            return pathFind != null && (GetCurrentState() != null || GetCurrentState().CanChangeState());
        }

        public bool CanChangeState()
        {
            return GetCurrentState() == null || states.Peek().CanInterrupt();
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
            // todo: set a flag when finished spawning
            return !IsDead();
        }

        public bool IsEngaged()
        {
            // todo: check this is legit
            return owner.currentMainState == SetActorStatePacket.MAIN_STATE_ACTIVE && owner.target != null;
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

        public void InternalChangeTarget(Character target)
        {
            // todo: use invalid target id
            // todo: this is retarded, call entity's changetarget function
            owner.target = target;
            owner.currentLockedTarget = target != null ? target.actorId : 0xC0000000;
            owner.currentTarget = target != null ? target.actorId : 0xC0000000;

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
                ForceChangeState(new AttackState(this.owner, target));
                return true;
            }
            return false;
        }

        public void InternalDisengage()
        {
            pathFind?.Clear();
            GetTargetFind()?.Reset();

            owner.updateFlags |= ActorUpdateFlags.HpTpMp;

            // todo: use the update flags
            owner.ChangeState(SetActorStatePacket.MAIN_STATE_PASSIVE);

            ChangeTarget(null);
            ClearStates();
        }

        public void InternalAbility(Character target, uint abilityId)
        {

        }

        public void InternalCast(Character target, uint spellId)
        {
            ChangeState(new MagicState(owner, target, (ushort)spellId));
        }

        public void InternalWeaponSkill(Character target, uint weaponSkillId)
        {
            ChangeState(new WeaponSkillState(owner, target, (ushort)weaponSkillId));
        }

        public void InternalMobSkill(Character target, uint mobSkillId)
        {

        }

        public void InternalDie(DateTime tick, uint timeToFadeout)
        {
            ClearStates();
            Disengage();
            ForceChangeState(new DeathState(owner, tick, timeToFadeout));
        }

        public void InternalRaise(Character target)
        {
            // todo: place at target
            // ForceChangeState(new RaiseState(target));
        }
    }
}
