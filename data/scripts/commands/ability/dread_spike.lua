require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --Need a better way to do this
    
    for i = 223212,223217 do
        local remAction = caster.statusEffects.RemoveStatusEffectForBattleAction(i, 30329)

        if remAction ~= nil then
            actionContainer.AddAction(remAction);
            skill.statusTier = 2;
            break;
        end

    end

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;