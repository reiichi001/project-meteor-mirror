require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--Increased Damage and reduced recast time in place of stun
function onCombo(caster, target, spell)
    spell.castTimeMs = spell.castTimeMs / 2;
end;

function onMagicFinish(caster, target, spell, action)
    magic.onMagicFinish(caster, target, spell, action)
end;