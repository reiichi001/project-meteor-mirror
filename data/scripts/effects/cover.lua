require("global")
require("modifiers")
require("hiteffect")
require("utils")

--Restores 25% of damage taken as MP. Does not send a message
function onDamageTaken(effect, attacker, defender, action, actionContainer)
    defender.AddMP(0.25 * action.amount);
end;