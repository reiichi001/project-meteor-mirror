require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

--Idea: add way to sort list of targets by hp here?
function onMagicStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;