require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--Increased critical damage
function onCombo(caster, target, spell)
    spell.critDamageModifier = 1.5;
end;

function onMagicFinish(caster, target, spell, action)
    magic.onMagicFinish(caster, target, spell, action)
end;