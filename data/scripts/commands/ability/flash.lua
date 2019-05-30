require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27161: Enhanced Flash: Adds Blind effect to flash
    if caster.HasTrait(27161) then
        ability.statusChance = 1;
    end

    --27162: Enhanced Flash II: Expands Flash to affect enemies near target
    if caster.HasTrait(27162) then
        ability.aoeTarget = TargetFindAOEType.Circle;
    end

    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    action.enmity = 400;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;