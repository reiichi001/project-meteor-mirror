require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onMagicFinish(caster, target, spell, action)
    magic.onBuffMagicFinish(caster, target, spell, action)
end;