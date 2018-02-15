-- todo: add enums for status effects in global.lua
require("global")
require("battleutils")
--[[
     statId - see BattleTemp.cs
     modifier - Modifier.Intelligence, Modifier.Mind (see Modifier.cs)
     multiplier - 
  ]]
weaponskill = 
{

}

function HandleHealingSkill(caster, target, skill, action, statId, modifierId, multiplier, baseAmount)
    potency = potency or 1.0;
    healAmount = baseAmount;
    
    -- todo: shit based on mnd
    local mind = caster.GetMod(Modifier.Mind);
end;

function HandleAttackSkill(caster, target, skill, action, statId, modifierId, multiplier, baseAmount)
    -- todo: actually handle this
    damage = baseAmount or math.random(1,10) * 10;
    
    return damage;
end;

function HandleStoneskin(caster, target, skill, action, statId, modifierId, damage)
    --[[
    if target.statusEffects.HasStatusEffect(StatusEffect.Stoneskin) then
        -- todo: damage reduction
        return true;
    end;
    ]]
    return false;
end;

--The default onskillfinish for weaponskills.
function weaponskill.onSkillFinish(caster, target, skill, action)
    action.battleActionType = BattleActionType.AttackPhysical;
    local damage = math.random(50, 150);
    action.amount = damage;

    action.CalcHitType(caster, target, skill);
    action.TryStatus(caster, target, skill);
    return action.amount;
end;