require("global");
require("Ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onAbilityFinish(caster, target, ability, action)
    return onStatusAbilityFinish(caster, target, ability, action)
end;