require("modifiers")

--Antagonize's enmity bonus is x1.5 (works for both abilities and damage dealt). x1.65 when AF is worn.
--Is does this mean it's 1.5* or 2.5*?
function onCommandStart(effect, owner, skill, actionContainer)
    local enmityModifier = 1.5

    if effect.GetTier() == 2 then
        enmityModifier = 1.65;
    end

    skill.enmityModifier = skill.enmityModifier + enmityModifier;
end