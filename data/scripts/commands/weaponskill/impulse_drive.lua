require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Increased damage
function onPositional(caster, target, skill)
    skill.potency = skill.potency * 1.25
end;

--Increased crit hit rating
function onCombo(caster, target, skill)
    skill.critRateModifier = 1.25;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;