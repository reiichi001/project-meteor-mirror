require("modifiers")
require("battleutils")

--Forces crit of a single WS action from rear.
function onMagicCast(caster, effect, skill)
    skill.mpCost = skill.mpCost / 2;
end;

function onHit(effect, attacker, defender, action, actionContainer)
    if action.commandType == CommandType.Spell then
        --Parsimony returns 35% of damage done rounded up as MP.
        local mpToReturn = math.ceil(0.35 * action.amount);
        attacker.AddMp(mpToReturn);
        actionContainer.AddMPAction(attacker.actorId, 33007, mpToReturn);
        actionContainer.AddAction(attacker.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end