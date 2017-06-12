using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.ai.state;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.packets.send.actor;

// port of ai code in dsp by kjLotus
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

        public AIContainer(Character actor, Controller controller, PathFind pathFind, TargetFind targetFind)
        {
            this.owner = actor;
            this.states = new Stack<State>();
            this.controller = controller;
            this.pathFind = pathFind;
            this.targetFind = targetFind;
            latestUpdate = DateTime.Now;
            prevUpdate = latestUpdate;
        }

        public void Update(DateTime tick)
        {
            prevUpdate = latestUpdate;
            latestUpdate = tick;

            // todo: trigger listeners

            // todo: action queues


        }

        public void Engage(Character target)
        {
            if (controller != null)
                controller.Engage(target);
            else
                InternalEngage(target);
        }

        public bool IsEngaged()
        {
            // todo: check this is legit
            return owner.currentMainState == SetActorStatePacket.MAIN_STATE_ACTIVE;
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

        }

        public void ChangeController(Controller controller)
        {
            this.controller = controller;
        }

        public bool CanChangeState()
        {
            return states.Count == 0 || states.First().CanInterrupt();
        }

        public void ChangeState(State state)
        {
            if (states.Count < 10)
            {
                states.Push(state);
            }
            else
            {
                throw new Exception("shit");
            }
        }

        public void InternalEngage(Character target)
        {

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
    }
}
