using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.packets.send.actor;

// port of dsp's ai code https://github.com/DarkstarProject/darkstar/blob/master/src/map/ai/

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    // https://github.com/Windower/POLUtils/blob/master/PlayOnline.FFXI/Enums.cs
    [Flags]
    public enum ValidTarget : ushort
    {
        None = 0x00,
        Self = 0x01,
        Player = 0x02,
        PartyMember = 0x04,
        Ally = 0x08,
        NPC = 0x10,
        Enemy = 0x20,
        Unknown = 0x40,
        Object = 0x60,
        CorpseOnly = 0x80,
        Corpse = 0x9D // CorpseOnly + NPC + Ally + Partymember + Self
    }

    /// <summary> Targeting from/to different entity types </summary>
    enum TargetFindCharacterType : byte
    {
        None,
        /// <summary> Player can target all <see cref="Player">s in party </summary>
        PlayerToPlayer,
        /// <summary> Player can target all <see cref="BattleNpc"/>s (excluding player owned <see cref="Pet"/>s) </summary>
        PlayerToBattleNpc,
        /// <summary> BattleNpc can target other <see cref="BattleNpc"/>s </summary>
        BattleNpcToBattleNpc,
        /// <summary> BattleNpc can target <see cref="Player"/>s and their <see cref="Pet"/>s </summary>
        BattleNpcToPlayer,
    }

    /// <summary> Type of AOE region to create </summary>
    enum TargetFindAOEType : byte
    {
        None,
        /// <summary> Really a cylinder, uses extents parameter in SetAOEType </summary>
        Circle,
        /// <summary> Create a cone with angle in radians </summary>
        Cone,
        /// <summary> Box using self/target coords and </summary>
        Box
    }

    /// <summary> Set AOE around self or target </summary>
    enum TargetFindAOETarget : byte
    {
        /// <summary> Set AOE's origin at target's position </summary>
        Target,
        /// <summary> Set AOE's origin to own position. </summary>
        Self
    }

    /// <summary> Target finding helper class </summary>
    class TargetFind
    {
        private Character owner;
        private Character target;
        private Character masterTarget; // if target is a pet, this is the owner
        private TargetFindCharacterType findType;
        private ValidTarget validTarget;
        private TargetFindAOEType aoeType;
        private Vector3 targetPosition;
        private float extents;
        private float angle;
        private List<Character> targets;

        public TargetFind(Character owner)
        {
            this.owner = owner;
            Reset();
        }

        public void Reset()
        {
            this.target = null;
            this.findType = TargetFindCharacterType.None;
            this.validTarget = ValidTarget.Enemy;
            this.aoeType = TargetFindAOEType.None;
            this.targetPosition = null;
            this.extents = 0.0f;
            this.angle = 0.0f;
            this.targets = new List<Character>();
        }

        public List<T> GetTargets<T>() where T : Character
        {
            return new List<T>(targets.OfType<T>());
        }

        public List<Character> GetTargets()
        {
            return targets;
        }

        /// <summary>
        /// Call this before <see cref="FindWithinArea"/> <para/>
        /// </summary>
        /// <param name="extents">
        /// <see cref="TargetFindAOEType.Circle"/> - radius of circle <para/>
        /// <see cref="TargetFindAOEType.Cone"/> - height of cone <para/>
        /// <see cref="TargetFindAOEType.Box"/> - width of box / 2 (todo: set box length not just between user and target)
        /// </param>
        /// <param name="angle"> Angle in radians of cone </param>
        public void SetAOEType(ValidTarget validTarget, TargetFindAOEType aoeType, float extents = -1.0f, float angle = -1.0f)
        {
            this.validTarget = validTarget;
            this.aoeType = aoeType;
            this.extents = extents != -1.0f ? extents : 0.0f;
            this.angle = angle != -1.0f ? angle : 0.0f;
        }

        public void SetAOEBox(ValidTarget validTarget, float length, float width)
        {
            this.validTarget = validTarget;
            this.aoeType = TargetFindAOEType.Box;
            float x = owner.positionX - (float)Math.Cos(owner.rotation + (float)(Math.PI / 2)) * (length);
            float z = owner.positionZ + (float)Math.Sin(owner.rotation + (float)(Math.PI / 2)) * (length);
            this.targetPosition = new Vector3(x, owner.positionY, z);
            this.extents = width;
        }

        /// <summary>
        /// Find and try to add a single target to target list
        /// </summary>
        public void FindTarget(Character target, ValidTarget flags)
        {
            validTarget = flags;
            this.target = null;
            // todo: maybe this should only be set if successfully added?
            this.targetPosition = target.GetPosAsVector3();
            AddTarget(target, false);
        }

        /// <summary>
        /// <para> Call SetAOEType before calling this </para>
        /// Find targets within area set by <see cref="SetAOEType"/>
        /// </summary>
        public void FindWithinArea(Character target, ValidTarget flags)
        {
            validTarget = flags;
            // todo: maybe we should keep a snapshot which is only updated on each tick for consistency
            // are we creating aoe circles around target or self
            if ((aoeType & TargetFindAOEType.Circle) != 0 && validTarget != ValidTarget.Self)
                this.targetPosition = owner.GetPosAsVector3();
            else
                this.targetPosition = target.GetPosAsVector3();

            masterTarget = TryGetMasterTarget(target) ?? target;

            // todo: should i set this yet or wait til checked if valid target
            this.target = target;

            // todo: this is stupid
            bool withPet = (flags & ValidTarget.Ally) != 0 || masterTarget.allegiance != owner.allegiance;

            if (masterTarget != null)
                targets.Add(masterTarget);

            if (IsPlayer(owner))
            {
                if (masterTarget is Player)
                {
                    findType = TargetFindCharacterType.PlayerToPlayer;

                    // todo: handle player parties
                    if (masterTarget.currentParty != null)
                    {
                        if ((validTarget & ValidTarget.Ally) != 0)
                            AddAllInAlliance(masterTarget, withPet);
                        else
                            AddAllInParty(masterTarget, withPet);
                    }
                    else
                    {
                        AddTarget(masterTarget, withPet);
                    }
                }
                else
                {
                    findType = TargetFindCharacterType.PlayerToBattleNpc;
                    AddAllBattleNpcs(masterTarget, false);
                }
            }
            else
            {
                // todo: this needs checking..
                if (masterTarget is Player || owner.allegiance == CharacterTargetingAllegiance.Player)
                    findType = TargetFindCharacterType.BattleNpcToPlayer;
                else
                    findType = TargetFindCharacterType.BattleNpcToBattleNpc;

                // todo: configurable pet aoe buff
                if (findType == TargetFindCharacterType.BattleNpcToBattleNpc && TryGetMasterTarget(target) != null)
                    withPet = true;

                // todo: does ffxiv have call for help flag?
                //if ((findFlags & ValidTarget.HitAll) != 0)
                //{
                //    AddAllInZone(masterTarget, withPet);
                //}

                AddAllInAlliance(target, withPet);

                if (findType == TargetFindCharacterType.BattleNpcToPlayer)
                {
                    if (owner.allegiance == CharacterTargetingAllegiance.Player)
                        AddAllInZone(masterTarget, withPet);
                    else
                        AddAllInHateList();
                }
            }
            
        }

        /// <summary>
        /// Find targets within a box using owner's coordinates and target's coordinates as length
        /// with corners being `extents` yalms to either side of self and target
        /// </summary>
        private bool IsWithinBox(Character target, bool withPet)
        {
            var myPos = owner.GetPosAsVector3();
            var angle = Vector3.GetAngle(myPos, targetPosition);

            // todo: actually check this works..
            var myCorner = myPos.NewHorizontalVector(angle, extents);
            var myCorner2 = myPos.NewHorizontalVector(angle, -extents);

            var targetCorner = targetPosition.NewHorizontalVector(angle, extents);
            var targetCorner2 = targetPosition.NewHorizontalVector(angle, -extents);

            return target.GetPosAsVector3().IsWithinBox(targetCorner2, myCorner);
        }

        private bool IsWithinCone(Character target, bool withPet)
        {
            // todo:
            return false;
        }

        private void AddTarget(Character target, bool withPet)
        {
            if (CanTarget(target))
            {
                // todo: add pets too
                targets.Add(target);
            }
        }

        private void AddAllInParty(Character target, bool withPet)
        {
            // todo:
            /*
             * foreach (var actor in target.currentParty.GetMembers())
             * {
             *     AddTarget(actor, withPet);
             * }
             */
            AddTarget(target, withPet);
        }

        private void AddAllInAlliance(Character target, bool withPet)
        {
            // todo:
            /*
             * foreach (var actor in target.currentParty.GetAllianceMembers())
             * {
             *     AddTarget(actor, withPet);
             * }
             */
            AddTarget(target, withPet);
        }

        private void AddAllBattleNpcs(Character target, bool withPet)
        {
            var actors = owner.zone.GetActorsAroundActor<BattleNpc>(owner, 50);

            // todo: should we look for Characters instead in case player is charmed by BattleNpc
            foreach (BattleNpc actor in actors)
            {
                // todo:
                AddTarget(actor, false);
            }
        }

        private void AddAllInZone(Character target, bool withPet)
        {
            var actors = owner.zone.GetAllActors<Character>();
            foreach (Character actor in actors)
            {
                AddTarget(actor, withPet);
            }
        }

        private void AddAllInHateList()
        {
            if (!(owner is BattleNpc))
            {
                Program.Log.Error($"TargetFind.AddAllInHateList() owner [{owner.actorId}] {owner.customDisplayName} {owner.actorName} is not a BattleNpc");
            }
            else
            {
                foreach (var hateEntry in ((BattleNpc)owner).hateContainer.GetHateList())
                {
                    AddTarget(hateEntry.Value.actor, false);
                }
            }
        }

        public bool CanTarget(Character target, bool withPet = false, bool retarget = false)
        {
            // already targeted, dont target again
            if (target == null || !retarget && targets.Contains(target))
                return false;

            // cant target dead
            if ((validTarget & ValidTarget.Corpse) == 0 && target.IsDead())
                return false;

            bool targetingPlayer = target.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER;

            if ((validTarget & ValidTarget.Ally) != 0 && target.currentSubState != owner.currentSubState)
                return false;

            if ((validTarget & ValidTarget.Enemy) != 0 && target.currentSubState != (owner.currentSubState == SetActorStatePacket.SUB_STATE_MONSTER ?
                SetActorStatePacket.SUB_STATE_PLAYER : SetActorStatePacket.SUB_STATE_MONSTER))
                return false;

            if ((validTarget & ValidTarget.NPC) != 0 && target.currentSubState != SetActorStatePacket.SUB_STATE_NONE)
                return false;

            // todo: why is player always zoning?
            // cant target if zoning
            if (targetingPlayer && ((Player)target).playerSession.isUpdatesLocked)
            {
                owner.aiContainer.ChangeTarget(null);
                return false;
            }

            if (/*target.isZoning || owner.isZoning || */target.zone != owner.zone)
                return false;

            if (validTarget == ValidTarget.Self && aoeType != TargetFindAOEType.None && owner != target)
                return false;

            // hit everything within zone or within aoe region
            if (extents == 255.0f || aoeType == TargetFindAOEType.Circle && target.GetPosAsVector3().IsWithinCircle(targetPosition ?? target.GetPosAsVector3(), extents))
                return true;

            if (aoeType == TargetFindAOEType.Cone && IsWithinCone(target, withPet))
                return true;

            if (aoeType == TargetFindAOEType.Box && IsWithinBox(target, withPet))
                return true;

            if (aoeType == TargetFindAOEType.None && targets.Count != 0)
                return false;

            return true;
        }

        private bool IsPlayer(Character target)
        {
            if (target is Player)
                return true;

            // treat player owned pets as players too
            return TryGetMasterTarget(target) is Player;
        }

        private Character TryGetMasterTarget(Character target)
        {
            // if character is a player owned pet, treat as a player
            if (target.aiContainer != null)
            {
                var controller = target.aiContainer.GetController();
                if (controller != null && controller is PetController)
                    return ((PetController)controller).GetPetMaster();
            }
            return null;
        }

        private bool IsBattleNpcOwner(Character target)
        {
            // i know i copied this from dsp but what even
            if (owner.currentSubState != SetActorStatePacket.SUB_STATE_PLAYER || target.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER)
                return true;

            // todo: check hate list
            if (owner.currentSubState == SetActorStatePacket.SUB_STATE_MONSTER && ((BattleNpc)owner).hateContainer.GetMostHatedTarget() != target)
            {
                return false;
            }
            return false;
        }

        public Character GetValidTarget(Character target, ValidTarget findFlags)
        {
            if (target == null || target.currentSubState == SetActorStatePacket.SUB_STATE_PLAYER && ((Player)target).playerSession.isUpdatesLocked)
                return null;

            if ((findFlags & ValidTarget.Ally) != 0)
            {
                return owner.pet;
            }

            // todo: this is beyond retarded
            var oldFlags = this.validTarget;
            this.validTarget = findFlags;
            if (CanTarget(target, false, true))
            {
                this.validTarget = oldFlags;
                return target;
            }
            this.validTarget = oldFlags;

            return null;
        }
    }
}
