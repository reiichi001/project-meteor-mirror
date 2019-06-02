require("modifiers")
require("battleutils")

--This is the traited version of Decoy. It can also evade physical attacks.
function onPreAction(effect, caster, target, skill, action, actionContainer)
    --Evade single ranged or magic attack
    --Traited allows for physical attacks
    if  target.allegiance != caster.allegiance and (skill.isRanged or skill.GetActionType() == ActionType.Magic or skill.GetActionType() == ActionType.Physical) then
        --Set action's hit rate to 0
        action.hitRate = 0.0;
        action.resistRate = 400;
        --Remove status and add message
        defender.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end

end;