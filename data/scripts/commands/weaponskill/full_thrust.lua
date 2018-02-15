require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

function onSkillFinish(caster, target, skill, action)    
    caster.AddTP(1000);
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;