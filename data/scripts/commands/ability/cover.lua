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
    if caster.HasItemEquippedInSlot(8032701, 10) then
        coverTier = 2;
    end

    caster.statusEffects.AddStatusEffect(223063, coverTier, 0, 15, 0);

    --Apply Covered to target
    action.DoAction(caster, target, skill, actionContainer);
end;