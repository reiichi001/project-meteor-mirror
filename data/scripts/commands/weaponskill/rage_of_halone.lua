require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Accuracy increase
function onCombo(caster, target, skill)
    --Rage of Halone normally has a -40% hit rate modifier.
    --Does the combo negate that, or does it make it even more accurate than if it didnt have the modifier?
    skill.accuracyModifier = 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;