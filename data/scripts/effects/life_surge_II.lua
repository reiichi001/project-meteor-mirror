require("battleutils")

function onHit(effect, attacker, defender, action, actionContainer)
    if action.commandType == CommandType.AutoAttack then
        local healPercent = 0.20;

        if effect.GetTier() == 2 then
            healPercent = 0.40;
        end

        local amount = math.floor((healPercent * action.amount) + 1);
        attacker.AddHP(amount);
        actionContainer.AddHPAction(defender.actorId, 30332, amount);
    end
end;