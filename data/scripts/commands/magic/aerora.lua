require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--Increased damage and conversion to single target
function onCombo(caster, target, spell)
    spell.aoeType = 0;
    spell.potency = spell.potency * 1.5;
end;

function onMagicFinish(caster, target, spell, action)
    local damage = math.random(10, 100);
    
    --Dispels an effect on each target.
    local effects = target.statusEffects.GetStatusEffectsByFlag(16); --lose on dispel
    if effects != nil then
        target.statusEffects.RemoveStatusEffect(effects[0]);
    end;
    return damage;
end;