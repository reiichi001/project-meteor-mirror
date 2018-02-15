require("global");
require("weaponskill");

function onSkillPrepare(caster, target, spell)
    return 0;
end;

function onSkillStart(caster, target, spell)
    return 0;
end;

--Increased enmity
function onCombo(caster, target, skill)
    skill.enmityModifier = 1.5;
end;

function onSkillFinish(caster, target, skill, action)
    return weaponskill.onSkillFinish(caster, target, skill, action);
end;