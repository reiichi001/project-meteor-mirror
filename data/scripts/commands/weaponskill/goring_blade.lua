require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Chance to increase defense when executed from behind the target
function onPositional(caster, target, skill)
    skill.statusChance = 0.90;
end;

--Increases bleed damage
function onCombo(caster, target, skill)
    skill.statusTier = 2;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;