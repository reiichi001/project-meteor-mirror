require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Only the bard gets the Battle Voice effect
    if caster == target then
        actionContainer.AddAction(caster.statusEffects.AddStatusForBattleAction(223253, 1, 0, 30));
    end

    action.DoAction(caster, target, skill, actionContainer);
end;