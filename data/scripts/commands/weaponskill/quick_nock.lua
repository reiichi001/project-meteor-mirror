require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--fivefold attack/conversion
function onCombo(caster, target, skill)
    skill.numHits = 5;
    skill.aoeType = 0;
    skill.aoeTarget = 2;
    --animation change?
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;