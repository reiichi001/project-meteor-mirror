require("modifiers")
require("battleutils")

--Dread spike completely nullifies a physical action and absorbs how much damage it would have done (when it's powered up)
--I'm going to assume it only absorbs half damage without LS/PS up
--When I say it nullifies an attack, it even gets rid of the message. It's as if the damage action didn't happen. 
--It still shows the enemy's "Enemy used [command]." message but there is no 0 damage dealt message.
--Don't know how this works with multi-hit attacks or even how it works with stoneskin or other buffs that respond to damage
-- I dont really know how this should work...
function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    if skill.GetActionType() == ActionType.Physical then
        --maybe this works?
        local absorbPercent = 0.5;

        if effect.GetTier() == 2 then
            absorbPercent = 1;
        end

        local absorbAmount = action.amount * absorbPercent;
        action.amount = 0;
        action.worldMasterTextId = 0;

        defender.AddHP(absorbAmount);
        --30451: You recover [absorbAmount] HP.
        actionContainer.AddHPAction(defender.actorId, 30451, absorbAmount)
        --Dread Spike is lost after absorbing hp
        defender.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end;