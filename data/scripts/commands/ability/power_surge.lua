require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27281: Enhanced Power Surge: Increases effect of Power Surge by 50%
    if caster.HasTrait(27281) then
        ability.statusTier = 2;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Need a better way to do this
    actionContainer.AddAction(caster.statusEffects.RemoveStatusEffectForBattleAction(223215));
    actionContainer.AddAction(caster.statusEffects.RemoveStatusEffectForBattleAction(223216));
    actionContainer.AddAction(caster.statusEffects.RemoveStatusEffectForBattleAction(223217));

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;