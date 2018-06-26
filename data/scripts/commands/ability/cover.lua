require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --This is for the "Cover" effect the caster receives.
    local coverTier = 1
    --8032701: Gallant Surcoat: Enhances Cover
    if caster.GetEquipment().GetItemAtSlot(10).itemId == 8032701 then
        coverTier = 2;
    end

    actionContainer.AddAction(caster.statusEffects.AddStatusForBattleAction(223063, coverTier, skill.statusDuration));

    --Apply Covered to target
    action.DoAction(caster, target, skill, actionContainer);
end;