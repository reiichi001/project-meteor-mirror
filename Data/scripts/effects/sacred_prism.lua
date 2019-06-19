require("modifiers")
require("battleutils")

--Cure, Cura, Regen, Esuna, Enhancing spells (Hardcoded as Stoneskin and Sanguine since we dont have a good way to check what's an enhancing spell)
supportedSpells = [27346, 27347, 27358, 27357, 27350, 27307]

function onMagicCast(effect, caster, skill)
    if supportedSpells[skill.id] then
        skill.castTimeMs = skill.castTimeMs * 1.5;
        skill.aoeType = TargetFindAOEType.Circle;
        skill.aoeRange = 15;
    end
end

function onCommandFinish(effect, owner, skill, actionContainer)
    if supportedSpells[skill.id] then
        owner.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end;