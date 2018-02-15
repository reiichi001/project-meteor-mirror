require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Chance to render target unable to use weaponskills (pacification)
function onPositional(caster, target, skill)
    skill.statusChance = 0.50;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;