require("modifiers")
require("battleutils")

--Dread spike completely nullifies a physical action and absorbs how much damage it would have done (when it's powered up)
--I'm going to assume it only absorbs half damage without LS/PS up
--When I say it nullifies an attack, it even gets rid of the message. It's as if the attack didn't happen
--Don't know how this works with multi-hit attacks or even how it works with stoneskin or other buffs that respond to damage
-- I dont really know how this should work...
function onDamageTaken(effect, attacker, defender, action, actionContainer)
    if action.actionType == ActionType.Physical then
        --maybe this works?
        local absorbAmount = action.amount;
        action.amount = 0;
        action.worldMasterTextId = 0;

        attacker.AddHP(absorbAmount);
        --30451: You recover [absorbAmount] HP.
        actionContainer.AddHPAction(defender.actorId, 30451, absorbAmount)
        --Dread Spike is lost after absorbing hp
        actionContainer.AddAction(defender.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end;

