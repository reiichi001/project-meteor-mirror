require("modifiers")
require("battleutils")

function onHit(effect, attacker, defender, action, actionContainer)
    if action.commandType == CommandType.Spell then
        --Necrogenesis returns 75% of damage done rounded up(?) as MP.
        local hpToReturn = math.ceil(0.75 * action.amount);
        attacker.AddMp(hpToReturn);
        actionContainer.AddHPAction(attacker.actorId, 33012, mpToReturn);
        actionContainer.AddAction(attacker.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end