require("global")
require("modifiers")
require("hiteffect")
require("utils")

function onHit(effect, attacker, defender, action, actionContainer)
    local enmity = action.enmity;
    action.enmity = 0;

    defender.hateContainer.UpdateHate(effect.GetSource(), enmity);
    --Does collusion send a message?
    actionContainer.AddAction(attacker.statusEffects.RemoveStatusEffectForBattleAction(effect));
end;