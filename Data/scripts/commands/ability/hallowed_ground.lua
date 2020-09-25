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

--Get all targets with hate on caster and spread 1140 enmity between them.
function onSkillFinish(caster, target, skill, action, actionContainer)
    --[[
    local enemies = caster.GetTargetsWithHate()
    local enmity = 1140 / enemies.Count
    for enemy in enemies do
        enemy.hateContainer.updateHate(enmity);
    end]]

    
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;