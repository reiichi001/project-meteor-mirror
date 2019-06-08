require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27245: Swift Chameleon
    if caster.HasTrait(27245) then
        ability.recastTimeMs = ability.recastTimeMs - 60000;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Need a way to get all targets with hate for player
    --target.hateContainer.UpdateHate(caster, -840);
   action.DoAction(caster, target, skill, actionContainer); 
end;