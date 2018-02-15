require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Inflicts additional damage when bleed ends
--Note for later, going to set bleed tier to 2, when bleed get scripted, check if tier is 2 and add additional damage at the end
function onCombo(caster, target, skill)
    skill.statusTier = 2;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;