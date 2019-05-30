require ("global")

require ("ally")

function onSpawn(ally)
    ally:SetMaxHP(69420)
    ally:SetHP(ally:GetMaxHP())
    ally.isAutoAttackEnabled = false
    ally.neutral = false
end

function onCombatTick(ally, target, tick, contentGroupCharas)	
    allyGlobal.onCombatTick(ally, target, tick, contentGroupCharas)
end

function tryAggro(ally, contentGroupCharas)
    allyGlobal.tryAggro(ally, contentGroupCharas)
end

function onRoam(ally, contentGroupCharas)
    ally.detectionType = 0xFF
    ally.isMovingToSpawn = false
    ally.neutral = false
    ally.animationId = 0
    --allyGlobal.onCombatTick(ally, contentGroupCharas)
end