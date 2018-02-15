require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Chance to inflict blind on flank
function onPositional(caster, target, skill)
    skill.statusChance = 0.50;
    skill.statusDuration = 10;
end;

function onCombo(caster, target, skill)
    skill.basePotency = skill.basePotency * 1.5;
end;


function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;