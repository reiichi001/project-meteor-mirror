require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Is this before or after status is gained?
    --Will probably need to switch to a flag for this because it might include more than just these 3 effects.
    caster.statusEffects.RemoveStatusEffect(228011, actionContainer, 30329);
    caster.statusEffects.RemoveStatusEffect(228013, actionContainer, 30329);
    caster.statusEffects.RemoveStatusEffect(228021, actionContainer, 30329);

    action.DoAction(caster, target, skill, actionContainer);
end;