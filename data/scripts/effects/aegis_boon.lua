require("modifiers")
require("utils")

--Forces a full block (0 damage taken)
function onPreAction(effect, caster, target, skill, action, actionContainer)
    --Can aegis boon block rear attacks or non-physical attacks?
    action.blockRate = 100.0;
end;

--Heals for the amount of HP blocked, up to a certain point. I don't know what determines the cap but it seems to be 703 at level 50. Unsure if it scales down based on level, dlvl, or if that's an arbitrary cap added.
function onBlock(effect, attacker, defender, skill, action, actionContainer)
    --Amount blocked
    local absorbAmount = math.Clamp(action.amountMitigated, 0, 703);

    --33008: You recover x HP from Aegis Boon
    defender.AddHP(absorbAmount);
    actionContainer.AddHPAction(defender.actorId, 33008, absorbAmount);
    defender.statusEffects.RemoveStatusEffect(effect, actionContainer);
end;