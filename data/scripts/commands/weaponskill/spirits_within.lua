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
    skill.enmityModifier = 2.5;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Increased damage with higher hp
    --random guess
    local potencyModifier = caster:GetHPP() + 25;

    skill.basePotency = skill.basePotency * potencyModifier;

    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;