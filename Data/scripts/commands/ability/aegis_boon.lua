require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27164: Swift Aegis Boon
    if caster.HasTrait(27164) then
        ability.recastTimeMs = ability.recastTimeMs - 15000;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    action.DoAction(caster, target, skill, actionContainer);
end;