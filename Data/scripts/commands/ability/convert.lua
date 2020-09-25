require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    newMP = math.min(caster.GetHP(), caster.GetMaxMP())
    newHP = math.min(caster.GetMP(), caster.GetMaxHP())
    caster.SetHP(newHP)
    caster.SetMP(newMP)

    --Set effect id
    action.DoAction(caster, target, skill, actionContainer);
end;