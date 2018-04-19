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
    actionContainer.AddAction(caster.statusEffects.RemoveStatusEffectForBattleAction(228011));
    actionContainer.AddAction(caster.statusEffects.RemoveStatusEffectForBattleAction(228013));
    actionContainer.AddAction(caster.statusEffects.RemoveStatusEffectForBattleAction(228021));

    action.DoAction(caster, target, skill, actionContainer);
end;