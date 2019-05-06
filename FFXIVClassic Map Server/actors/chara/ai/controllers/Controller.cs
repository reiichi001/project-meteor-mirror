using System;
using FFXIVClassic_Map_Server.Actors;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    abstract class Controller
    {
        protected Character owner;

        protected DateTime lastCombatTickScript;
        protected DateTime lastUpdate;
        public bool canUpdate = true;
        protected bool autoAttackEnabled = true;
        protected bool castingEnabled = true;
        protected bool weaponSkillEnabled = true;
        protected PathFind pathFind;
        protected TargetFind targetFind;

        public Controller(Character owner)
        {
            this.owner = owner;
        }

        public abstract void Update(DateTime tick);
        public abstract bool Engage(Character target);
        public abstract void Cast(Character target, uint spellId);
        public virtual void WeaponSkill(Character target, uint weaponSkillId) { }
        public virtual void MonsterSkill(Character target, uint mobSkillId) { }
        public virtual void UseItem(Character target, uint slot, uint itemId) { }
        public abstract void Ability(Character target, uint abilityId);
        public abstract void RangedAttack(Character target);
        public virtual void Spawn() { }
        public virtual void Despawn() { }


        public virtual void Disengage()
        {
            owner.aiContainer.InternalDisengage();
        }

        public virtual void ChangeTarget(Character target)
        {
            owner.aiContainer.InternalChangeTarget(target);
        }

        public bool IsAutoAttackEnabled()
        {
            return autoAttackEnabled;
        }

        public void SetAutoAttackEnabled(bool isEnabled)
        {
            autoAttackEnabled = isEnabled;
        }

        public bool IsCastingEnabled()
        {
            return castingEnabled;
        }

        public void SetCastingEnabled(bool isEnabled)
        {
            castingEnabled = isEnabled;
        }

        public bool IsWeaponSkillEnabled()
        {
            return weaponSkillEnabled;
        }

        public void SetWeaponSkillEnabled(bool isEnabled)
        {
            weaponSkillEnabled = isEnabled;
        }
    }
}
