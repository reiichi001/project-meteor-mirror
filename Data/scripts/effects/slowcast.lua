require("modifiers")
require("battleutils")

--Increases range of a single spell, no clue by how much, 25% is a random guess
--It isn't clear if it has an effect on the aoe portion of skills or just the normal range, i've seen people on the OF say both.
--It also increased height of skills
function onMagicCast(effect, caster, skill)
    skill.castTimeMs = skill.castTimeMs * 1.5;
end;