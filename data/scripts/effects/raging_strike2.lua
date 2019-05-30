require("modifiers")
require("battleutils")

function onHit(effect, attacker, defender, skill, action, actionContainer)
    if skill.id == 27259 then
        --Effect stacks up to 3 times
        if effect.GetTier() < 3 then
            effect.SetTier(effect.GetTier() + 1);
        end
    end
end

function onMiss(effect, attacker, defender, skill, action, actionContainer)
    if skill.id == 27259 then
        
    end
end