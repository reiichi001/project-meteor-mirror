require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--http://forum.square-enix.com/ffxiv/threads/41900-White-Mage-A-Guide read
function onMagicFinish(caster, target, spell, action)
    magic.onCureMagicFinish(caster, target, spell, action)
end;