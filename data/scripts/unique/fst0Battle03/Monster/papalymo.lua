require ("global")
require ("modifiers")
require ("ally")

function onSpawn(ally)
    ally:SetMaxHP(69420)
    ally:SetHP(ally:GetMaxHP())
    ally.isAutoAttackEnabled = false;
    ally.neutral = false
    ally:SetMod(modifiersGlobal.Speed, 0)
end