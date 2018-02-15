require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Increased accuracy
function onCombo(caster, target, skill)
    skill.accuracyModifier = 1;
end;
    
--Increased damage
function onCombo(caster, target, skill)
    skill.potency = skill.potency * 1.5;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;