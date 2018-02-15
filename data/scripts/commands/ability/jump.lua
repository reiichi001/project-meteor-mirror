require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onAbilityFinish(caster, target, skill, action)
    return onAttackAbilityFinish(caster, target, skill, action);
end;