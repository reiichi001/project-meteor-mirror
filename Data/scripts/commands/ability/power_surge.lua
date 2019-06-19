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
    caster.statusEffects.RemoveStatusEffect(223215);
    caster.statusEffects.RemoveStatusEffect(223216);
    caster.statusEffects.RemoveStatusEffect(223217);

    --If caster has any of the power surge effects
    local buff = caster.statusEffects.GetStatusEffectById(223212) or caster.statusEffects.GetStatusEffectById(223213) or caster.statusEffects.GetStatusEffectById(223214);

    if buff ~= nil then
        caster.statusEffects.RemoveStatusEffect(buff, actionContainer, 30329);
    else
        --DoAction handles rates, buffs, dealing damage
        action.DoAction(caster, target, skill, actionContainer);
    end
end;