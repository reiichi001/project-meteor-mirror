require("modifiers")
require("battleutils")

--This is the traited version of Decoy. It can also evade physical attacks.
function onPreAction(effect, caster, target, skill, action, actionContainer)
    --Evade single ranged or magic attack
    --Traited allows for physical attacks
    if  target.allegiance != caster.allegiance and (skill.isRanged or action.actionType == ActionType.Magic or action.actionType == ActionType.Physical) then
        --Set action's hit rate to 0
        action.hirRate = 0.0;
        --Remove status and add message
        actionContainer.AddAction(target.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end

end;