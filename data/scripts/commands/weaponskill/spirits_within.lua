require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Increased enmity
function onCombo(caster, target, skill)
    skill.enmityModifier = 1.5;
end;

function onSkillFinish(caster, target, skill, action)
    local damage = math.random(10, 100);
    --Increased damage with higher hp
    local potencyModifier = caster:GetHPP() + 25;
    skill.basePotency = skill.basePotency * (potencyModifier / 100);
    return weaponskill.onSkillFinish(caster, target, skill, action)
end;