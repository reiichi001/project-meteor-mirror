require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --8032701: Fighter's Gauntlets: Reduces Collusion cooldown by 10 seconds
    if caster.HasItemEquippedInSlot(8032701, 13) then
        skill.recastTimeMs = skill.recastTimeMs - 10000;
    end

    action.DoAction(caster, target, skill, actionContainer);
end;