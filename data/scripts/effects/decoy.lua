require("modifiers")
require("battleutils")

--This is the untraited version of decoy.

function onPreAction(caster, target, effect, skill, action, actionContainer)
    --Evade single ranged or magic attack
    --Traited allows for physical attacks
    if skill.isRanged or action.actionType == ActionType.Magic then
        --Set action's hit rate to 0
        action.hirRate = 0.0;
    end

    --Remove status and add message 
    actionsList.AddAction(target.statusEffects.RemoveForBattleAction(effect));
end;