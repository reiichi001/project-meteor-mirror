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
        private PathFind pathFind;
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

            // todo: action queues
            controller?.Update(tick);
            State currState;
            while (states.Count > 0 && (currState = states.Peek()).Update(tick))
            {
                if (currState  == GetCurrentState())
                {

                }
            }
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

        public bool CanChangeState()
        {
            return states.Count == 0 || states.Peek().CanInterrupt();
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
            if (GetCurrentState() != null)
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

        public State GetCurrentState()
        {
            return states.Peek() ?? null;
        }

        public DateTime GetLatestUpdate()
        {
            return latestUpdate;
        }

        public bool IsSpawned()
        {
            // todo: set a flag when finished spawning
            return true;
        }

        public bool IsEngaged()
        {
            // todo: check this is legit
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

        }

        public void InternalCast(Character target, uint spellId)
        {

        }

        public void InternalWeaponSkill(Character target, uint weaponSkillId)
        {

        }

        public void InternalMobSkill(Character target, uint mobSkillId)
        {

        }

        public void InternalDie(DateTime tick, uint timeToFadeout)
        {

        }

        public void InternalRaise(Character target)
        {
            // todo: place at target
            // ForceChangeState(new RaiseState(target));
        }
    }
}
