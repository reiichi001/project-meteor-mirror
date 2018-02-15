-- todo: add enums for status effects in global.lua
require("global")
require("battleutils")

magic =
{
    
};

--[[
     statId - see BattleTemp.cs
     modifierId - Modifier.Intelligence, Modifier.Mind (see Modifier.cs)
     multiplier - 
  ]]
function magic.HandleHealingMagic(caster, target, spell, action, statId, modifierId, multiplier, baseAmount)
    potency = potency or 1.0;
    healAmount = baseAmount;
    
    -- todo: shit based on mnd
    local mind = caster.GetMod(Modifier.Mind);
end;

function magic.HandleAttackMagic(caster, target, spell, action, statId, modifierId, multiplier, baseAmount)
    -- todo: actually handle this
    damage = baseAmount or math.random(1,10) * 10;
    
    return damage;
end;

function magic.HandleEvasion(caster, target, spell, action, statId, modifierId)

    return false;
end;

function magic.HandleStoneskin(caster, target, spell, action, statId, modifierId, damage)
    --[[
    if target.statusEffects.HasStatusEffect(StatusEffect.Stoneskin) then
        -- todo: damage reduction
        return true;
    end;
    ]]
    return false;
end;

function magic.onMagicFinish(caster, target, spell, action)
    action.battleActionType = BattleActionType.AttackMagic;
    local damage = math.random(50, 150);
    action.amount = damage;
    action.CalcHitType(caster, target, spell);
    action.TryStatus(caster, target, spell, true);
    return action.amount;
end;

--For healing magic
function magic.onCureMagicFinish(caster, target, spell, action)
    action.battleActionType = BattleActionType.Heal;
    local amount = math.random(200, 450);
    action.amount = amount;
    action.CalcHitType(caster, target, spell);
    action.TryStatus(caster, target, spell, true);
    return action.amount;
end;

--For status magic
function magic.onStatusMagicFinish(caster, target, spell, action)
    action.battleActionType = BattleActionType.Status;
    action.amount = 0;
    action.CalcHitType(caster, target, spell);
    action.TryStatus(caster, target, spell, false);
    return action.amount;
end;