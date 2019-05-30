require("global");
require("ability");


function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Need a better way to do this
    
    for i = 223212,223217 do
        local buff = caster.statusEffects.GetStatusEffectById(i);

        if buff ~= nil then
            caster.statusEffects.RemoveStatusEffect(buff, actionContainer, 30329);
            skill.statusTier = 2;
            break;
        end

    end

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;