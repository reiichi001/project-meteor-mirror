require("global");
require("ability");

function onAbilityPrepare(caster, target, skill)
    return 0;
end;

function onAbilityStart(caster, target, skill)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    action.amount = skill.basePotency;
    
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;