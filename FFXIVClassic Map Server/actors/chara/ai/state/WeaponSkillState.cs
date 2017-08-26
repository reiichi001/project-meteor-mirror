using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.packets.send;

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

            // todo: check recast
            if (owner.CanWeaponSkill(target, skill, ref errorPacket))
            {
                // todo: Azia can fix, check the recast time and send error
                OnStart();
            }
            else if (interrupt || errorPacket != null)
            {
                if (owner is Player && errorPacket != null)
                    ((Player)owner).QueuePacket(errorPacket);

                errorPacket = null;
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaAbilityFunction(owner, skill, "weaponskills", "onSkillStart", owner, target, skill);

            if (returnCode != 0)
            {
                interrupt = true;
                errorPacket = BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, owner.actorId, 0, 0, (ushort)(returnCode == -1 ? 32558 : returnCode), skill.id, 0, 1);
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

            var targets = skill.targetFind.GetTargets();
            BattleAction[] actions = new BattleAction[targets.Count];
            List<SubPacket> packets = new List<SubPacket>();

            var i = 0;
            foreach (var chara in targets)
            {
                var action = new BattleAction();
                action.effectId = 0;
                action.param = 1;
                action.unknown = 1;
                action.targetId = chara.actorId;
                action.worldMasterTextId = skill.worldMasterTextId;
                action.amount = (ushort)lua.LuaEngine.CallLuaAbilityFunction(owner, skill, "skills", "onSkillFinish", owner, target, skill, action);
                actions[i++] = action;

                //packets.Add(BattleActionX01Packet.BuildPacket(chara.actorId, owner.actorId, action.targetId, skill.battleAnimation, action.effectId, action.worldMasterTextId, skill.id, action.amount, action.param));
            }
            packets.Add(BattleActionX10Packet.BuildPacket(owner.target.actorId, owner.actorId, skill.battleAnimation, skill.id, actions));
            owner.zone.BroadcastPacketsAroundActor(owner, packets);
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
            return owner.CanWeaponSkill(target, skill, ref errorPacket);
        }
    }
}
