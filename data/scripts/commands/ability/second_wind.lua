require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onAbilityFinish(caster, target, ability, action)
    return onHealAbilityFinish(caster, target, ability, action)
end;