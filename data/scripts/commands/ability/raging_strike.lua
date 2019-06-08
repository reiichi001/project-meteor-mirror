require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    ability.statusMagnitude = 100;
    --27243: Enhanced Raging Strike: Increases effect of Raging Strike by 50%
    if caster.HasTrait(27241) then
        ability.statusMagnitude = ability.statusMagnitude * 1.5;
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --If caster has raging strike, remove it and send message, otherwise apply it.
    local buff = caster.statusEffects.GetStatusEffectById(223221)

    if buff ~= nil then
        --30329: Your Raging Strike removes your Raging Strike effect.
        caster.statusEffects.RemoveStatusEffect(buff, actionContainer, 30329);
    else
        --DoAction handles rates, buffs, dealing damage
        action.DoAction(caster, target, skill, actionContainer);
    end
end;