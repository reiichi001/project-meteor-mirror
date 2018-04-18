require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27283: Enhanced Blood for Blood: Increases damage dealt to enemies by B4B by 25%
    if caster.HasTrait(27283) then
        ability.statusTier = 2;
    end

    --27284: Swift Blood for Blood: Reduces recast time of B4B by 15 seconds
    if caster.HasTrait(27284) then
        ability.recastTimeMs = ability.recastTimeMs - 15000;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;