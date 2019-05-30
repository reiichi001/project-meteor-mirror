require("global");
require("weaponskill");
require("utils");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Increased accuracy
function onCombo(caster, target, skill)
    skill.accuracyModifier = 50;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = 5000;--skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;