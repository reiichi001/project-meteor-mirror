require("modifiers")
require("battleutils")

function onHit(effect, attacker, defender, skill, action, actionContainer)
    if skill.GetCommandType() == CommandType.Spell then
        --Necrogenesis returns 75% of damage done rounded up(?) as MP.
        local hpToReturn = math.ceil(0.75 * action.amount);
        attacker.AddHP(hpToReturn);
        actionContainer.AddHPAbsorbAction(defender.actorId, 33012, hpToReturn);
        attacker.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end