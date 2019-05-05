require("modifiers")
require("battleutils")

--Increases range of a single spell, no clue by how much, 25% is a random guess
--It isn't clear if it has an effect on the aoe portion of skills or just the normal range, i've seen people on the OF say both.
function onMagicStart(caster, effect, skill)
    skill.range = skill.range * 1.25;
end;

--The effect falls off after the skill is finished, meaning if you start a cast and cancel, it shouldn't fall off.
function onCommandFinish(effect, owner, skill, actionContainer)
    if action.commandType == CommandType.Spell then
        actionContainer.AddAction(owner.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end