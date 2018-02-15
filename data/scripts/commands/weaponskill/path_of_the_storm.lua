require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Chance to inflict heavy when executed from behind
function onPositional(caster, target, skill)
    skill.statusChance = 0.50;
    skill.statusDuration = 5;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;