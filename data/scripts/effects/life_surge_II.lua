require("battleutils")

function onHit(effect, attacker, defender, skill, action, actionContainer)
    if skill.GetCommandType() == CommandType.AutoAttack then
        local healPercent = 0.20;

        if effect.GetTier() == 2 then
            healPercent = 0.40;
        end

        local amount = math.floor((healPercent * action.amount) + 1);
        attacker.AddHP(amount);
        actionContainer.AddHPAbsorbAction(defender.actorId, 30332, amount);
    end
end;