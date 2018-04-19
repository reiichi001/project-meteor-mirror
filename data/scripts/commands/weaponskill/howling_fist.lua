require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Increased accuracy
function onPositional(caster, target, skill)
    skill.accuracyModifier = 50;
end;
    
--Increased damage
function onCombo(caster, target, skill)
    skill.basePotency = skill.basePotency * 1.5;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;