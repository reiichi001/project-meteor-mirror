require("modifiers")
require("battleutils")

--Unclear what the exact damage is but it seems like it's the total amount of damage the attack would have done before parrying + 1
function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    local amount = action.amount + action.amountMitigated + 1;

    --Only reflects magical attacks if wearing AF chest
    if skill.GetActionType() == ActionType.Physical or (skill.GetActionType() == ActionType.Magic and effect.GetTier() == 2) then
        --30350: Counter! You hit target for x points of damage
        --There are counter messages for blocks, can Vengeance be blocked/parried?
        attacker.DelHP(amount, actionContainer);
        actionContainer.AddHitAction(attacker.actorId, 30350, amount);
    end;
end;