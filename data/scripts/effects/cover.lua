require("modifiers")

--Enahnced Cover: Restores 25% of damage taken as MP. Does not send a message
function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    if effect.GetTier() == 2 then
        defender.AddMP(0.25 * action.amount);
    end
end;