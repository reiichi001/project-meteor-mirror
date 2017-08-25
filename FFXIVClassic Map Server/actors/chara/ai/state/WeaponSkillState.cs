using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class WeaponSkillState : State
    {

        private Ability skill;

        public WeaponSkillState(Character owner, Character target, ushort skillId) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            // todo: lookup skill from global table
            this.skill = Server.GetWorldManager().GetAbility(skillId);
            var returnCode = lua.LuaEngine.CallLuaAbilityFunction(owner, skill, "weaponskills", "onSkillPrepare", owner, target, skill);

            if (skill != null && returnCode == 0)
            {
                // todo: Azia can fix, check the recast time and send error

                if (!skill.IsValidTarget(owner, target))
                {
                    // todo: error message
                    interrupt = true;
                }
                else if ((skill.tpCost = (ushort)Math.Ceiling((8000 + (owner.charaWork.parameterSave.state_mainSkillLevel - 70) * 500) * (skill.tpCost * 0.001))) > owner.GetTP())
                {
                    // todo: error message
                    interrupt = true;
                }
                else if (skill.level > owner.charaWork.parameterSave.state_mainSkillLevel)
                {
                    // todo: error message
                }
                else if (false /*skill.requirements & */)
                {
                    // todo: error message
                }
                else
                {
                    OnStart();
                }
            }
            else
            {
                if (owner is Player)
                    ((Player)owner).SendGameMessage(Server.GetWorldManager().GetActor(), (ushort)(returnCode == -1 ? 32539 : returnCode), 0x20);
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaAbilityFunction(owner, skill, "weaponskills", "onSkillStart", owner, target, skill);

            if (returnCode != 0)
            {
                interrupt = true;
                errorPacket = BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, owner.actorId, 0, 0, (ushort)returnCode, skill.id, 0, 1);
            }
            else
            {
                // todo: check within attack range
                owner.LookAt(target);
            }
        }

        public override bool Update(DateTime tick)
        {
            if (skill != null)
            {
                TryInterrupt();

                if (interrupt)
                {
                    OnInterrupt();
                    return true;
                }

                // todo: check weapon delay/haste etc and use that
                var actualCastTime = skill.castTimeSeconds;

                if ((tick - startTime).TotalSeconds >= skill.castTimeSeconds)
                {
                    OnComplete();
                    return true;
                }
                return false;
            }
            return true;
        }

        public override void OnInterrupt()
        {
            // todo: send paralyzed/sleep message etc.
            if (errorPacket != null)
            {
                owner.zone.BroadcastPacketAroundActor(owner, errorPacket);
            }
        }

        public override void OnComplete()
        {
            skill.targetFind.FindWithinArea(target, skill.validTarget);
            isCompleted = true;

            List<SubPacket> packets = new List<SubPacket>();
            foreach (var chara in skill.targetFind.GetTargets())
            {
                // todo: calculate shit, do shit
                bool landed = true;
                var amount = lua.LuaEngine.CallLuaAbilityFunction(owner, skill, "weaponskills", "onSkillFinish", owner, target, skill);

                foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                {
                    player.QueuePacket(BattleActionX01Packet.BuildPacket(player.actorId, owner.actorId, chara.actorId, skill.battleAnimation, skill.effectAnimation, skill.worldMasterTextId, skill.id, (ushort)skill.param, 1));
                }

                if (chara is BattleNpc)
                {
                    ((BattleNpc)chara).hateContainer.UpdateHate(owner, amount);
                }
            }

        }

        public override void TryInterrupt()
        {
            if (interrupt)
                return;

            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction))
            {
                // todo: sometimes paralyze can let you attack, get random percentage of actually letting you attack
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction);
                uint effectId = 0;
                if (list.Count > 0)
                {
                    // todo: actually check proc rate/random chance of whatever effect
                    effectId = list[0].GetStatusEffectId();
                }
                // todo: which is actually the swing packet
                //this.errorPacket = BattleActionX01Packet.BuildPacket(target.actorId, owner.actorId, target.actorId, 0, effectId, 0, (ushort)BattleActionX01PacketCommand.Attack, (ushort)damage, 0);
                //owner.zone.BroadcastPacketAroundActor(owner, errorPacket);
                //errorPacket = null;
                interrupt = true;
                return;
            }

            interrupt = !CanUse();
        }

        private bool CanUse()
        {
            if (target == null)
            {
                return false;
            }
            // todo: shouldnt need to check if owner is dead since all states would be cleared
            if (owner.aiContainer.IsDead() || target.aiContainer.IsDead())
            {
                return false;
            }
            else if (!owner.aiContainer.GetTargetFind().CanTarget(target, false, true))
            {
                return false;
            }
            else if (Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ) > skill.range)
            {
                if (owner.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER)
                {
                    ((Player)owner).SendGameMessage(Server.GetWorldManager().GetActor(), 32539, 0x20);
                }
                return false;
            }
            return true;
        }
    }
}
