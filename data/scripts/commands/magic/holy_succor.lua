require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onMagicFinish(caster, target, spell, action)
    spell.statusId = 228011;
    spell.statusDuration = 25;
    spell.statusChance = 1.0;
    magic.onCureMagicFinish(caster, target, spell, action)

    if caster != target then
        action.AddHealAction(caster.actorId, (action.amount / 2));
    end
end;