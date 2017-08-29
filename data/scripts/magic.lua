-- todo: add enums for status effects in global.lua
require("global")

magic =
{
    
};

--[[
     modifier - Modifier.Intelligence, Modifier.Mind (see Modifier.cs)
     multiplier - 
  ]]
function magic.HandleHealingMagic(caster, target, spell, action, modifierId, multiplier, baseAmount)
    potency = potency or 1.0;
    healAmount = baseAmount;
    
    -- todo: shit based on mnd
    local mind = caster.GetMod(Modifier.Mind);
end;

function magic.HandleAttackMagic(caster, target, spell, action, modifierId, multiplier, baseAmount)
    -- todo: actually handle this
    damage = baseAmount or math.random(1,10) * 10;
    
    return damage;
end;

function magic.HandleEvasion(caster, target, spell, action, modifierId)

    return false;
end;

function magic.HandleStoneskin(caster, target, spell, action, modifierId, damage)
    --[[
    if target.statusEffects.HasStatusEffect(StatusEffect.Stoneskin) then
        -- todo: damage reduction
        return true;
    end;
    ]]
    return false;
end;