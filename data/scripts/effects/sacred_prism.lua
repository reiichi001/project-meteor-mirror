require("modifiers")
require("battleutils")

function onMagicCast(caster, effect, skill)
    skill.aoeType = TargetFindAOEType.Circle;
    skill.aoeRange = 15;
    skill.validTarget = 31
end