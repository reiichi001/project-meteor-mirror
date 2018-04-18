require("global")
require("modifiers")
require("hiteffect")
require("utils")

--Unclear what the exact damage is but it seems like it's the total amount of damage the attack would have done before parrying
function onDamageTaken(effect, attacker, defender, action, actionContainer)
    local amount = action.amount + action.mitigatedAmount;

    --Only reflects magical attacks if wearing AF chest
    if action.actionType == ActionType.Physical or (action.actionType == ActionType.Magic and effect.GetTier() == 2) then
        --30350: Counter! You hit target for x points of damage
        --There are counter messages for blocks, can Vengeance be blocked/parried?
        attacker.DelHP(amount);
        actionContainer.AddHitAction(attacker.actorId, 30350, amount);
    end;
end;