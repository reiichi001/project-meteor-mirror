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
    class MagicState : State
    {

        private Ability spell;
        private uint cost;
        private Vector3 startPos;

        public MagicState(Character owner, Character target, ushort spellId) :
            base(owner, target)
        {
            this.startTime = DateTime.Now;
            // todo: lookup spell from global table
            this.spell = Server.GetWorldManager().GetAbility(spellId);
            var returnCode = lua.LuaEngine.CallLuaAbilityFunction(owner, spell, "spells", "onSpellPrepare", owner, target, spell);

            if (spell != null && returnCode == 0)
            {
                // todo: hp/mp shit should be taken care of in scripts, not here
                // todo: Azia can fix, check the recast time and send error

                if (!spell.IsValidTarget(owner, target))
                {
                    // todo: error message
                    interrupt = true;
                }
                else if ((spell.mpCost = (ushort)Math.Ceiling((8000 + (owner.charaWork.parameterSave.state_mainSkillLevel - 70) * 500) * (spell.mpCost * 0.001))) > owner.GetMP())
                {
                    // todo: error message
                    interrupt = true;
                }
                else if (spell.level > owner.charaWork.parameterSave.state_mainSkillLevel)
                {
                    // todo: error message
                }
                else if (false /*spell.requirements & */)
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
                // todo: fuckin retarded. enum log messages somewhere (prolly isnt even right param)
                if (owner is Player)
                ((Player)owner).SendGameMessage(Server.GetWorldManager().GetActor(), (ushort)(returnCode == -1 ? 32539 : returnCode), 0x20);
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaAbilityFunction(owner, spell, "spells", "onSpellStart", owner, target, spell);

            if (returnCode != 0)
            {
                interrupt = true;
                errorPacket = BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, owner.actorId, 0, 0, (ushort)(returnCode == -1 ? 32539 : returnCode), spell.id, 0, 1);
            }
            else
            {
                // todo: check within attack range
                startPos = owner.GetPosAsVector3();
                owner.LookAt(target);

                foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                {
                    // todo: this is retarded, prolly doesnt do what i think its gonna do
                    player.QueuePacket(BattleActionX01Packet.BuildPacket(player.actorId, owner.actorId, target != null ? target.actorId : 0xC0000000, spell.battleAnimation, spell.effectAnimation, 0, spell.id, 0, (byte)spell.castTimeSeconds));
                }
            }
        }

        public override bool Update(DateTime tick)
        {
            if (spell != null)
            {
                TryInterrupt();

                if (interrupt)
                {
                    OnInterrupt();
                    return true;
                }

                // todo: check weapon delay/haste etc and use that
                var actualCastTime = spell.castTimeSeconds;

                if ((tick - startTime).TotalSeconds >= spell.castTimeSeconds)
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
            spell.targetFind.FindWithinArea(target, spell.validTarget);
            isCompleted = true;
            
            List<SubPacket> packets = new List<SubPacket>();
            foreach (var chara in spell.targetFind.GetTargets())
            {
                // todo: calculate shit, do shit
                bool landed = true;
                var amount = lua.LuaEngine.CallLuaAbilityFunction(owner, spell, "spells", "onSpellFinish", owner, target, spell);

                foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                {
                    player.QueuePacket(BattleActionX01Packet.BuildPacket(player.actorId, owner.actorId, chara.actorId, spell.battleAnimation, spell.effectAnimation, spell.worldMasterTextId, spell.id, (ushort)spell.param, 1));
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

            if (Utils.DistanceSquared(owner.GetPosAsVector3(), startPos) > 4.0f)
            {
                // todo: send interrupt packet
                interrupt = true;
                return;
            }
            
            interrupt = !CanCast();
        }

        private bool CanCast()
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
            else if (Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ) > spell.range)
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
