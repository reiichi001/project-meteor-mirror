require("global");
require("ability");

function onAbilityPrepare(caster, target, skill)
    return 0;
end;

function onAbilityStart(caster, target, skill)
    return 0;
end;

function onAbilityFinish(caster, target, skill, action)
    return onStatusAbilityFinish(caster, target, skill, action);
end;