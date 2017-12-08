require ("modifiers")

function onSpawn(mob)
    mob:SetMod(modifiersGlobal.Speed, 0)
end

function onDamageTaken(mob, attacker, damage)
    if not attacker:IsPlayer() and mob:GetHP() - damage < 0 then
        mob:addHP(damage)
    end
end

function onCombatTick(mob, target, tick, contentGroupCharas)
    if mob:GetSpeed() == 0 then
        mob:SetMod(modifiersGlobal.Speed, 8)
    end
end

function onDisengage(mob)
    mob:SetMod(modifiersGlobal.Speed, 0)
    mob:Despawn()
end