-- todo: add enums for status effects in global.lua
require("global")

weaponskill =
{
    
};

--[[
     statId - see BattleTemp.cs
     modifier - Modifier.Intelligence, Modifier.Mind (see Modifier.cs)
     multiplier - 
  ]]
function weaponskill.HandleHealingSkill(caster, target, spell, action, statId, modifierId, multiplier, baseAmount)
    potency = potency or 1.0;
    healAmount = baseAmount;
    
    -- todo: shit based on mnd
    local mind = caster.GetMod(Modifier.Mind);
end;

function weaponskill.HandleAttackSkill(caster, target, spell, action, statId, modifierId, multiplier, baseAmount)
    -- todo: actually handle this
    damage = baseAmount or math.random(1,10) * 10;
    
    return damage;
end;

function weaponskill.HandleEvasion(caster, target, spell, action, statId, modifierId)

    return false;
end;

function weaponskill.HandleStoneskin(caster, target, spell, action, statId, modifierId, damage)
    --[[
    if target.statusEffects.HasStatusEffect(StatusEffect.Stoneskin) then
        -- todo: damage reduction
        return true;
    end;
    ]]
    return false;
end;