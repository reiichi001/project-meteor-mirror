require ("global")

require ("ally")

function onSpawn(ally)
    ally.isAutoAttackEnabled = false;
end;

function onCombatTick(ally, target, tick, contentGroupCharas)	
    allyGlobal.onCombatTick(ally, target, tick, contentGroupCharas);
end;