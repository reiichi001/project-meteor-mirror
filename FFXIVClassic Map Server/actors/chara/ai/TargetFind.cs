using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;

// port of dsp's ai code https://github.com/DarkstarProject/darkstar/blob/master/src/map/ai/

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    /// <summary> todo: what even do i summarise this as? </summary>
    [Flags]
    enum TargetFindFlags
    {
        None,
        /// <summary> Able to target <see cref="Player"/>s even if not in target's party </summary>
        All,
        /// <summary> Able to target all <see cref="Player"/>s in target's party/alliance </summary>
        Alliance,
        /// <summary> Able to target any <see cref="Pet"/> in target's party/alliance </summary>
        Pets,
        /// <summary> Target all in zone, regardless of distance </summary>
        ZoneWide,
        /// <summary> Able to target dead <see cref="Player"/>s </summary>
        Dead,
    }

    /// <summary> Targeting from/to different entity types </summary>
    enum TargetFindCharacterType
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
    enum TargetFindAOEType
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
    enum TargetFindAOERadiusType
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
        private TargetFindFlags findFlags;
        private TargetFindAOEType aoeType;
        private TargetFindAOERadiusType radiusType;
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
            this.findFlags = TargetFindFlags.None;
            this.aoeType = TargetFindAOEType.None;
            this.radiusType = TargetFindAOERadiusType.Self;
            this.targetPosition = null;
            this.extents = 0.0f;
            this.angle = 0.0f;
            this.targets = new List<Character>();
        }

        /// <summary>
        /// Call this before <see cref="FindWithinArea"/> <para/>
        /// </summary>
        /// <param name="extents">
        /// <see cref="TargetFindAOEType.Circle"/> - radius of circle <para/>
        /// <see cref="TargetFindAOEType.Cone"/> - height of cone <para/>
        /// <see cref="TargetFindAOEType.Box"/> - width of box / 2
        /// </param>
        /// <param name="angle"> Angle in radians of cone </param>
        public void SetAOEType(TargetFindAOERadiusType radiusType, TargetFindAOEType aoeType, float extents = -1.0f, float angle = -1.0f)
        {
            this.radiusType = TargetFindAOERadiusType.Target;
            this.aoeType = aoeType;
            this.extents = extents != -1.0f ? extents : 0.0f;
            this.angle = angle != -1.0f ? angle : 0.0f;
        }

        /// <summary>
        /// Find and try to add a single target to target list
        /// </summary>
        public void FindTarget(Character target, TargetFindFlags flags)
        {
            findFlags = flags;
            this.target = null;
            // todo: maybe this should only be set if successfully added?
            this.targetPosition = target.GetPosAsVector3();
            AddTarget(target, false);
        }

        /// <summary>
        /// <para> Call SetAOEType before calling this </para>
        /// Find targets within area set by <see cref="SetAOEType"/>
        /// </summary>

        public void FindWithinArea(Character target, TargetFindFlags flags)
        {
            findFlags = flags;

            // todo: maybe we should keep a snapshot which is only updated on each tick for consistency
            // are we creating aoe circles around target or self
            if ((aoeType & TargetFindAOEType.Circle) != 0 && radiusType != TargetFindAOERadiusType.Self)
                this.targetPosition = owner.GetPosAsVector3();
            else
                this.targetPosition = target.GetPosAsVector3();

            masterTarget = GetMasterTarget(target);

            // todo: this is stupid
            bool withPet = (flags & TargetFindFlags.Pets) != 0 || masterTarget.currentSubState != owner.currentSubState;

            if (IsPlayer(owner))
            {
                if (masterTarget is Player)
                {
                    findType = TargetFindCharacterType.PlayerToPlayer;

                    // todo: handle player parties
                    if (masterTarget.currentParty != null)
                    {
                        if ((findFlags & TargetFindFlags.Alliance) != 0)
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
                    
                }
            }
            if (aoeType == TargetFindAOEType.Box)
            {
                FindWithinBox(withPet);
            }
            else if (aoeType == TargetFindAOEType.Circle)
            {
                FindWithinCircle(withPet);
            }
            else if (aoeType == TargetFindAOEType.Cone)
            {
                FindWithinCone(withPet);
            }
        }

        /// <summary>
        /// Find targets within a box using owner's coordinates and target's coordinates as length
        /// with corners being `extents` yalms to either side of self and target
        /// </summary>
        private void FindWithinBox(bool withPet)
        {
            // todo: loop over party members
            if ((findFlags & TargetFindFlags.All) != 0)
            {
                // if we have flag set to hit all characters in zone, do it

                // todo: make the distance check modifiable
                var actors = (findFlags & TargetFindFlags.ZoneWide) != 0 ? owner.zone.GetAllActors<Character>() : owner.zone.GetActorsAroundActor<Character>(owner, 70);
                var myPos = owner.GetPosAsVector3();
                var angle = Vector3.GetAngle(myPos, targetPosition);

                // todo: actually check this works..
                var myCorner = myPos.NewHorizontalVector(angle, extents);
                var myCorner2 = myPos.NewHorizontalVector(angle, -extents);

                var targetCorner = targetPosition.NewHorizontalVector(angle, extents);
                var targetCorner2 = targetPosition.NewHorizontalVector(angle, -extents);

                foreach (Character actor in actors.OfType<Character>())
                {
                    // dont wanna add static actors
                    if (actor is Player || actor is BattleNpc)
                    {
                        if (actor.GetPosAsVector3().IsWithinBox(targetCorner2, myCorner))
                        {
                            if (CanTarget(actor))
                                AddTarget(actor, withPet);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Find targets within circle area. <para/>
        /// As the name implies, it only checks horizontal coords, not vertical - 
        /// effectively creating cylinder with infinite height
        /// </summary>
        private void FindWithinCircle(bool withPet)
        {
            var actors = (findFlags & TargetFindFlags.ZoneWide) != 0 ? owner.zone.GetAllActors<Character>() : owner.zone.GetActorsAroundActor<Character>(owner, 70);

            foreach (Character actor in actors)
            {
                if (actor is Player || actor is BattleNpc)
                {
                    if (actor.GetPosAsVector3().IsWithinCircle(targetPosition, extents))
                        AddTarget(target, withPet);
                }
            }
        }

        private void FindWithinCone(bool withPet)
        {
            
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
            AddTarget(target, withPet);
        }

        private void AddAllInAlliance(Character target, bool withPet)
        {
            // todo:
            AddTarget(target, withPet);
        }

        public bool CanTarget(Character target)
        {
            // already targeted, dont target again
            if (targets.Contains(target))
                return false;

            // cant target dead
            if ((findFlags & TargetFindFlags.Dead) == 0 && target.IsDead())
                return false;

            // cant target if player is zoning
            if (target is Player && ((Player)target).playerSession.isUpdatesLocked)
                return false;



            return true;
        }

        private bool IsPlayer(Character target)
        {
            if (target is Player)
                return true;

            // treat player owned pets as players too
            return GetMasterTarget(target) is Player;
        }

        private Character GetMasterTarget(Character target)
        {
            // if character is a player owned pet, treat as a player
            if (target.aiContainer != null)
            {
                var controller = target.aiContainer.GetController();
                if (controller != null && controller is PetController)
                {
                    return ((PetController)controller).GetPetMaster();
                }
            }
            return target;
        }
    }
}
