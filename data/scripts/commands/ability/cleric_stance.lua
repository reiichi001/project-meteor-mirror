require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    local buff = caster.statusEffects.GetStatusEffectById(223227)

    if buff ~= nil then
        caster.statusEffects.RemoveStatusEffect(buff, actionContainer, 30329);
    else
        --DoAction handles rates, buffs, dealing damage
        action.DoAction(caster, target, skill, actionContainer);
    end
end;