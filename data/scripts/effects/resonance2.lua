require("modifiers")
require("battleutils")

--Increases range of a single spell, no clue by how much, 25% is a random guess
--It isn't clear if it has an effect on the aoe portion of skills or just the normal range, i've seen people on the OF say both.
--It also increased height of skills
function onMagicCast(effect, caster, skill)
    skill.range = skill.range * 1.25;
    skill.rangeHeight = skill.rangeHeight * 1.25;
end;

--The effect falls off after the skill is finished, meaning if you start a cast and cancel, it shouldn't fall off.
function onCommandFinish(effect, owner, skill, actionContainer)
    if skill.GetCommandType() == CommandType.Spell then
        owner.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end