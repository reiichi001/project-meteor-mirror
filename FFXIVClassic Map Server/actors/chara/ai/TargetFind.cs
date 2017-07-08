using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;

// port of dsp's ai code https://github.com/DarkstarProject/darkstar/blob/master/src/map/ai/

namespace FFXIVClassic_Map_Server.actors.chara.ai
{
    [Flags]
    enum TargetFindFlags : ushort
    {
        None,
        All,      // able to target players who arent in party
        Alliance, // alliance 
        Pet,      // allow targeting pets
        ZoneWide, // 
        Dead,     // allow targeting of dead players
    }

    enum TargetFindCharacterType
    {
        None,
        PlayerToPlayer,       // player can target all players in party
        PlayerToBattleNpc,    // player can target all battlenpc (excluding player owned pets)
        BattleNpcToBattleNpc, // battlenpc can target other battlenpcs
        BattleNpcToPlayer,    // battlenpc can target players
    }

    enum TargetFindAOEType
    {
        None,
        Circle,
        Cone,
        Box
    }

    enum TargetFindAOERadiusType
    {
        Target,
        Self
    }

    class TargetFind
    {
        private Character owner;
        private Character target;
        private TargetFindCharacterType findType;
        private TargetFindFlags findFlags;
        private TargetFindAOEType aoeType;
        private Vector3 targetPosition;
        private float range;
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
            this.targetPosition = null;
            this.range = 0.0f;
            this.angle = 0.0f;
        }

        public void SetAOEType(TargetFindAOEType type, float range = -1.0f, float angle = -1.0f)
        {
            aoeType = type;
            range = range != -1.0f ? range : 0.0f;
            angle = angle != -1.0f ? angle : 0.0f;
        }

        public void FindTarget(Character target, TargetFindFlags flags)
        {
            findFlags = flags;
            this.target = null;
            this.targetPosition = new Vector3(target.positionX, target.positionY, target.positionZ);

            AddTarget(target, false);
        }

        public void FindWithinArea(Character target, float radius, TargetFindFlags flags)
        {

        }

        private void AddTarget(Character target, bool withPet)
        {
            if (CanTarget(target))
                targets.Add(target);

            // todo: add pets too
        }

        private void AddAllInParty(Character target, bool withPet)
        {
            
        }

        private void AddAllInAlliance(Character target, bool withPet)
        {

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
    }
}
