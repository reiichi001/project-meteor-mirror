using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.actors.chara;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.actors.chara.ai.state;
using FFXIVClassic_Map_Server.utils;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.actors.chara.ai.utils;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.Actors.Chara;

namespace FFXIVClassic_Map_Server.Actors
{
    [Flags]
    enum DetectionType
    {
        None                  = 0x00,
        Sight                 = 0x01,
        Scent                 = 0x02,
        Sound                 = 0x04,
        LowHp                 = 0x08,
        IgnoreLevelDifference = 0x10,
        Magic                 = 0x20,
    }

    enum KindredType
    {
        Unknown   = 0,
        Beast     = 1,
        Plantoid  = 2,
        Aquan     = 3,
        Spoken    = 4,
        Reptilian = 5,
        Insect    = 6,
        Avian     = 7,
        Undead    = 8,
        Cursed    = 9,
        Voidsent  = 10,
    }

    class BattleNpc : Npc
    {
        public HateContainer hateContainer;
        public DetectionType detectionType;
        public KindredType kindredType;
        public bool neutral;
        protected uint despawnTime;
        protected uint respawnTime;
        protected uint spawnDistance;
        protected uint bnpcId;
        public Character lastAttacker;

        public uint spellListId, skillListId, dropListId;
        public Dictionary<uint, BattleCommand> skillList = new Dictionary<uint, BattleCommand>();
        public Dictionary<uint, BattleCommand> spellList = new Dictionary<uint, BattleCommand>();

        public uint poolId, genusId;
        public ModifierList poolMods;
        public ModifierList genusMods;
        public ModifierList spawnMods;

        protected Dictionary<MobModifier, Int64> mobModifiers = new Dictionary<MobModifier, Int64>();

        public BattleNpc(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot,
            ushort actorState, uint animationId, string customDisplayName)
            : base(actorNumber, actorClass, uniqueId, spawnedArea, posX, posY, posZ, rot, actorState, animationId, customDisplayName)  
        {
            this.aiContainer = new AIContainer(this, new BattleNpcController(this), new PathFind(this), new TargetFind(this));

            //this.currentSubState = SetActorStatePacket.SUB_STATE_MONSTER;
            //this.currentMainState = SetActorStatePacket.MAIN_STATE_ACTIVE;

            //charaWork.property[2] = 1;
            //npcWork.hateType = 1;
            
            this.hateContainer = new HateContainer(this);
            this.allegiance = CharacterTargetingAllegiance.BattleNpcs;

            spawnX = posX;
            spawnY = posY;
            spawnZ = posZ;

            despawnTime = 10;
            CalculateBaseStats();
        }

        public override List<SubPacket> GetSpawnPackets(Player player, ushort spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            if (IsAlive())
            {
                subpackets.Add(CreateAddActorPacket());
                subpackets.AddRange(GetEventConditionPackets());
                subpackets.Add(CreateSpeedPacket());
                subpackets.Add(CreateSpawnPositonPacket(0x0));

                subpackets.Add(CreateAppearancePacket());

                subpackets.Add(CreateNamePacket());
                subpackets.Add(CreateStatePacket());
                subpackets.Add(CreateSubStatePacket());
                subpackets.Add(CreateInitStatusPacket());
                subpackets.Add(CreateSetActorIconPacket());
                subpackets.Add(CreateIsZoneingPacket());
                subpackets.Add(CreateScriptBindPacket(player));
                subpackets.Add(GetHateTypePacket(player));
            }
            return subpackets;
        }

        //This might need more work
        //I think there migh be something that ties mobs to parties 
        //and the client checks if any mobs are tied to the current party
        //and bases the color on that. Adding mob to party obviously doesn't work
        //Based on depictionjudge script:
        //HATE_TYPE_NONE is for passive
        //HATE_TYPE_ENGAGED is for aggroed mobs
        //HATE_TYPE_ENGAGED_PARTY is for claimed mobs, client uses occupancy group to determine if mob is claimed by player's party
        //for now i'm just going to assume that occupancygroup will be BattleNpc's currentparties when they're in combat, 
        //so if party isn't null, they're claimed.
        public SubPacket GetHateTypePacket(Player player)
        {
            npcWork.hateType = NpcWork.HATE_TYPE_NONE;
            if (player != null)
            {
                if (aiContainer.IsEngaged())
                {
                    npcWork.hateType = NpcWork.HATE_TYPE_ENGAGED;

                    if (this.currentParty != null)
                    {
                        npcWork.hateType = NpcWork.HATE_TYPE_ENGAGED_PARTY;
                    }
                }
            }
            npcWork.hateType = 3;
            var propPacketUtil = new ActorPropertyPacketUtil("npcWork/hate", this);
            propPacketUtil.AddProperty("npcWork.hateType");
            return propPacketUtil.Done()[0];
        }

        public uint GetDetectionType()
        {
            return (uint)detectionType;
        }
        
        public void SetDetectionType(uint detectionType)
        {
            this.detectionType = (DetectionType)detectionType;
        }

        public override void Update(DateTime tick)
        {
            this.aiContainer.Update(tick);
            this.statusEffects.Update(tick);
        }

        public override void PostUpdate(DateTime tick, List<SubPacket> packets = null)
        {
            // todo: should probably add another flag for battleTemp since all this uses reflection
            packets = new List<SubPacket>();
            if ((updateFlags & ActorUpdateFlags.HpTpMp) != 0)
            {
                var propPacketUtil = new ActorPropertyPacketUtil("charaWork/stateAtQuicklyForAll", this);
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkill[0]");
                propPacketUtil.AddProperty("charaWork.parameterSave.state_mainSkillLevel");

                propPacketUtil.AddProperty("charaWork.battleTemp.castGauge_speed[0]");
                propPacketUtil.AddProperty("charaWork.battleTemp.castGauge_speed[1]");
                packets.AddRange(propPacketUtil.Done());
            }
            base.PostUpdate(tick, packets);
        }

        public override bool CanAttack()
        {
            // todo:
            return true;
        }

        public override bool CanCast(Character target, BattleCommand spell)
        {
            // todo:
            if (target == null)
            {
                // Target does not exist.
                return false;
            }
            if (Utils.Distance(positionX, positionY, positionZ, target.positionX, target.positionY, target.positionZ) > spell.range)
            {
                // The target is out of range.
                return false;
            }
            if (!IsValidTarget(target, spell.mainTarget) || !spell.IsValidMainTarget(this, target))
            {
                // error packet is set in IsValidTarget
                return false;
            }
            return true;
        }

        public override bool CanWeaponSkill(Character target, BattleCommand skill)
        {
            // todo:
            return true;
        }

        public override bool CanUseAbility(Character target, BattleCommand ability)
        {
            // todo:
            return false;
        }

        public uint GetDespawnTime()
        {
            return despawnTime;
        }

        public void SetDespawnTime(uint seconds)
        {
            despawnTime = seconds;
        }

        public uint GetRespawnTime()
        {
            return respawnTime;
        }

        public void SetRespawnTime(uint seconds)
        {
            respawnTime = seconds;
        }

        ///<summary> // todo: create an action object? </summary>
        public bool OnAttack(AttackState state)
        {
            return false;
        }

        public override void Spawn(DateTime tick)
        {
            if (respawnTime > 0)
            {
                ForceRespawn();
            }
        }

        public void ForceRespawn()
        {
            base.Spawn(Program.Tick);

            this.isMovingToSpawn = false;
            this.hateContainer.ClearHate();
            zone.BroadcastPacketsAroundActor(this, GetSpawnPackets(null, 0x01));
            zone.BroadcastPacketsAroundActor(this, GetInitPackets());
            charaWork.parameterSave.hp = charaWork.parameterSave.hpMax;
            charaWork.parameterSave.mp = charaWork.parameterSave.mpMax;
            RecalculateStats();

            OnSpawn();
            updateFlags |= ActorUpdateFlags.AllNpc;
        }

        public override void Die(DateTime tick, CommandResultContainer actionContainer = null)
        {
            if (IsAlive())
            {
                // todo: does retail 
                if (lastAttacker is Pet && lastAttacker.aiContainer.GetController<PetController>() != null && lastAttacker.aiContainer.GetController<PetController>().GetPetMaster() is Player)
                {
                    lastAttacker = lastAttacker.aiContainer.GetController<PetController>().GetPetMaster();
                }

                if (lastAttacker is Player)
                {
                    //I think this is, or should be odne in DoBattleAction. Packet capture had the message in the same packet as an attack
                    // <actor> defeat/defeats <target>
                    if (actionContainer != null)
                        actionContainer.AddEXPAction(new CommandResult(actorId, 30108, 0));

                    if (lastAttacker.currentParty != null && lastAttacker.currentParty is Party)
                    {
                        foreach (var memberId in ((Party)lastAttacker.currentParty).members)
                        {
                            var partyMember = zone.FindActorInArea<Character>(memberId);
                            // onDeath(monster, player, killer)
                            lua.LuaEngine.CallLuaBattleFunction(this, "onDeath", this, partyMember, lastAttacker);

                            // todo: add actual experience calculation and exp bonus values.
                            if (partyMember is Player)
                                BattleUtils.AddBattleBonusEXP((Player)partyMember, this, actionContainer);
                        }
                    }
                    else
                    {
                        // onDeath(monster, player, killer)
                        lua.LuaEngine.CallLuaBattleFunction(this, "onDeath", this, lastAttacker, lastAttacker);
                        //((Player)lastAttacker).QueuePacket(BattleActionX01Packet.BuildPacket(lastAttacker.actorId, 0, 0, new BattleAction(actorId, 30108, 0)));
                    }
                }

                if (positionUpdates != null)
                    positionUpdates.Clear();

                aiContainer.InternalDie(tick, despawnTime);
                //this.ResetMoveSpeeds();
                // todo: reset cooldowns

                lua.LuaEngine.GetInstance().OnSignal("mobkill");
            }
            else
            {
                var err = String.Format("[{0}][{1}] {2} {3} {4} {5} tried to die ded", actorId, GetUniqueId(), positionX, positionY, positionZ, GetZone().GetName());
                Program.Log.Error(err);
                //throw new Exception(err);
            }
        }

        public override void Despawn(DateTime tick)
        {
            // todo: probably didnt need to make a new state...
            aiContainer.InternalDespawn(tick, respawnTime);
            lua.LuaEngine.CallLuaBattleFunction(this, "onDespawn", this);
            this.isAtSpawn = true;
        }

        public void OnRoam(DateTime tick)
        {
            // leash back to spawn
            if (!IsCloseToSpawn())
            {
                if (!isMovingToSpawn)
                {
                    aiContainer.Reset();
                    isMovingToSpawn = true;
                }
                else
                {
                    if (target == null && !aiContainer.pathFind.IsFollowingPath())
                        aiContainer.pathFind.PathInRange(spawnX, spawnY, spawnZ, 1.5f, 15.0f);
                }
            }
            else
            {
                // recover hp
                if (GetHPP() < 100)
                {
                    AddHP(GetMaxHP() / 10);
                }
                else
                {
                    this.isMovingToSpawn = false;
                }
            }
        }

        public bool IsCloseToSpawn()
        {
            return this.isAtSpawn = Utils.DistanceSquared(positionX, positionY, positionZ, spawnX, spawnY, spawnZ) <= 2500.0f;
        }

        public override void OnAttack(State state, CommandResult action, ref CommandResult error)
        {
            base.OnAttack(state, action, ref error);
            // todo: move this somewhere else prolly and change based on model/appearance (so maybe in Character.cs instead)
            action.animation = 0x11001000; // (temporary) wolf anim

            if (GetMobMod((uint)MobModifier.AttackScript) != 0)
                lua.LuaEngine.CallLuaBattleFunction(this, "onAttack", this, state.GetTarget(), action.amount);
        }

        public override void OnCast(State state, CommandResult[] actions, BattleCommand spell, ref CommandResult[] errors)
        {
            base.OnCast(state, actions, spell, ref errors);

            if (GetMobMod((uint)MobModifier.SpellScript) != 0)
                foreach (var action in actions)
                    lua.LuaEngine.CallLuaBattleFunction(this, "onCast", this, zone.FindActorInArea<Character>(action.targetId), ((MagicState)state).GetSpell(), action);
        }

        public override void OnAbility(State state, CommandResult[] actions, BattleCommand ability, ref CommandResult[] errors)
        {
            base.OnAbility(state, actions, ability, ref errors);

            /*
            if (GetMobMod((uint)MobModifier.AbilityScript) != 0)
                foreach (var action in actions)
                    lua.LuaEngine.CallLuaBattleFunction(this, "onAbility", this, zone.FindActorInArea<Character>(action.targetId), ((AbilityState)state).GetAbility(), action);
            */
        }

        public override void OnWeaponSkill(State state, CommandResult[] actions, BattleCommand skill, ref CommandResult[] errors)
        {
            base.OnWeaponSkill(state, actions, skill, ref errors);

            if (GetMobMod((uint)MobModifier.WeaponSkillScript) != 0)
                foreach (var action in actions)
                    lua.LuaEngine.CallLuaBattleFunction(this, "onWeaponSkill", this, zone.FindActorInArea<Character>(action.targetId), ((WeaponSkillState)state).GetWeaponSkill(), action);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            lua.LuaEngine.CallLuaBattleFunction(this, "onSpawn", this);
        }

        public override void OnDeath()
        {
            base.OnDeath();
        }

        public override void OnDespawn()
        {
            base.OnDespawn();
        }

        public uint GetBattleNpcId()
        {
            return bnpcId;
        }

        public void SetBattleNpcId(uint id)
        {
            this.bnpcId = id;
        }

        public Int64 GetMobMod(MobModifier mobMod)
        {
            return GetMobMod((uint)mobMod);
        }

        public Int64 GetMobMod(uint mobModId)
        {
            Int64 res;
            if (mobModifiers.TryGetValue((MobModifier)mobModId, out res))
                return res;
            return 0;
        }

        public void SetMobMod(uint mobModId, Int64 val)
        {
            if (mobModifiers.ContainsKey((MobModifier)mobModId))
                mobModifiers[(MobModifier)mobModId] = val;
            else
                mobModifiers.Add((MobModifier)mobModId, val);
        }

        public override void OnDamageTaken(Character attacker, BattleCommand skill, CommandResult action,  CommandResultContainer actionContainer = null)
        {
            if (GetMobMod((uint)MobModifier.DefendScript) != 0)
                lua.LuaEngine.CallLuaBattleFunction(this, "onDamageTaken", this, attacker, action.amount);
            base.OnDamageTaken(attacker, skill, action, actionContainer);
        }
    }
}
