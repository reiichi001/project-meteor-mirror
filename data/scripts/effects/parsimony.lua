require("modifiers")
require("battleutils")

--Forces crit of a single WS action from rear.
function onMagicCast(effect, caster, skill)
    skill.mpCost = skill.mpCost / 2;
end;

--Having two identical functions seems weird. also don't know if parsimony still activates if resisted or evaded?
function onHit(effect, attacker, defender, skill, action, actionContainer)
    if skill.GetCommandType() == CommandType.Spell then
        local percent = 0.10;

        if effect.GetTier() == 2 then
            percent = 0.35;
        end

        --Parsimony returns 10% (35 traited) of damage done rounded up as MP.
        local mpToReturn = math.ceil(percent * action.amount);
        attacker.AddMP(mpToReturn);
        actionContainer.AddMPAbsorbAction(0, 33007, mpToReturn);
        defender.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end