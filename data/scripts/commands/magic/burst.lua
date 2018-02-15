require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--Increased damage with lesser current hp
function onCombo(caster, target, spell)

end;
    
function onMagicFinish(caster, target, spell, action)
    magic.onMagicFinish(caster, target, spell, action)
end;