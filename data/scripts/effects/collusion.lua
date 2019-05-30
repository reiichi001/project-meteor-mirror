require("modifiers")

function onHit(effect, attacker, defender, skill, action, actionContainer)
    local enmity = action.enmity;
    action.enmity = 0;

    defender.hateContainer.UpdateHate(effect.GetSource(), enmity);
    --Does collusion send a message?
    defender.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
end;