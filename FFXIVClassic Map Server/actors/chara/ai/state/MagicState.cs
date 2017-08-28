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
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class MagicState : State
    {

        private BattleCommand spell;
        private Vector3 startPos;

        public MagicState(Character owner, Character target, ushort spellId) :
            base(owner, target)
        {
            this.startPos = owner.GetPosAsVector3();
            this.startTime = DateTime.Now;
            // todo: lookup spell from global table
            this.spell = Server.GetWorldManager().GetAbility(spellId);
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicPrepare", owner, target, spell);

            // todo: check recast
            if (returnCode == 0 && owner.CanCast(target, spell, ref errorPacket))
            {
                // todo: Azia can fix, check the recast time and send error
                OnStart();
            }
            else
            {
                if (owner is Player)
                {
                    // "Your battle command fails to activate"
                    if (errorPacket == null)
                        errorPacket = owner.CreateGameMessagePacket(Server.GetWorldManager().GetActor(), (ushort)(returnCode == -1 ? 32410 : returnCode), 0x20, owner.actorId);

                    ((Player)owner).QueuePacket(errorPacket);
                }
                errorPacket = null;
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicStart", owner, target, spell);

            if (returnCode != 0)
            {
                interrupt = true;
                errorPacket = BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, owner.actorId, 0, 0, (ushort)(returnCode == -1 ? 32558 : returnCode), spell.id, 0, 1);
            }
            else
            {
                // todo: check within attack range
                float[] baseCastDuration = { 1.0f, 0.25f };

                float spellSpeed = spell.castTimeSeconds;
                List<SubPacket> packets = new List<SubPacket>();

                // command casting duration
                if (owner.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER)
                {
                    // todo: modify spellSpeed based on modifiers and stuff
                    // ((Player)owner).SendStartCastBar(spell.id, Utils.UnixTimeStampUTC(DateTime.Now.AddSeconds(spellSpeed)));

                }
                // todo: change 

                owner.zone.BroadcastPacketsAroundActor(owner, packets);
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
            
            var targets = spell.targetFind.GetTargets();
            BattleAction[] actions = new BattleAction[targets.Count];
            List<SubPacket> packets = new List<SubPacket>();
            var i = 0;
            foreach (var chara in targets)
            {
                var action = new BattleAction();
                action.effectId = spell.effectAnimation;
                action.param = 1;
                action.unknown = 1;
                action.targetId = chara.actorId;
                action.worldMasterTextId = spell.worldMasterTextId;
                action.amount = (ushort)lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicFinish", owner, chara, spell, action);
                actions[i++] = action;

                //packets.Add(BattleActionX01Packet.BuildPacket(chara.actorId, owner.actorId, action.targetId, spell.battleAnimation, action.effectId, action.worldMasterTextId, spell.id, action.amount, action.param));
            }
            owner.zone.BroadcastPacketAroundActor(owner,
                           spell.aoeType != TargetFindAOEType.None ? (BattleActionX10Packet.BuildPacket(owner.target.actorId, owner.actorId, spell.battleAnimation, spell.id, actions)) :
                           BattleActionX01Packet.BuildPacket(owner.actorId, owner.actorId, target.actorId, spell.battleAnimation, actions[0].effectId, actions[0].worldMasterTextId, spell.id, actions[0].amount, actions[0].param)
                           );
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
            
            interrupt = !CanCast();
        }

        private bool CanCast()
        {
            return owner.CanCast(target, spell, ref errorPacket) && !HasMoved();
        }

        private bool HasMoved()
        {
            return (Utils.DistanceSquared(owner.GetPosAsVector3(), startPos) > 4.0f);
        }

        public override void Cleanup()
        {
            // command casting duration
            var packets = new List<SubPacket>();
            if (owner.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER)
            {
                // ((Player)owner).SendStartCastBar(0, 0);
            }
            owner.zone.BroadcastPacketsAroundActor(owner, packets);
        }

        public BattleCommand GetSpell()
        {
            return spell;
        }
    }
}
