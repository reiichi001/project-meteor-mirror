require ("global")
require ("magic")
require ("weaponskill")

allyGlobal =
{
}

function allyGlobal.onSpawn(ally, target)

end

function allyGlobal.onEngage(ally, target)

end

function allyGlobal.onAttack(ally, target, damage)

end

function allyGlobal.onDamageTaken(ally, attacker, damage)

end

function allyGlobal.onCombatTick(ally, target, tick, contentGroupCharas)
    allyGlobal.HelpPlayers(ally, contentGroupCharas)
end

function allyGlobal.onDeath(ally, player, lastAttacker)

end

function allyGlobal.onDespawn(ally)

end

function allyGlobal.HelpPlayers(ally, contentGroupCharas, pickRandomTarget)
    print("helpPlayers");
    if contentGroupCharas and not ally.IsEngaged() then
        print("contentGroup exists");
        for chara in contentGroupCharas do
            print("looping");
            if chara then
                -- probably a player, or another ally
                -- todo: queue support actions, heal, try pull hate off player etc
                if chara.IsPlayer() then
                    print("chara is a player");
                    -- do stuff
                    if not ally.IsEngaged() then
                        if chara.IsEngaged() then
                            allyGlobal.EngageTarget(ally, chara.target, nil);
                            break;
                        end
                    end                    
                elseif chara.IsMonster() and chara.IsEngaged() then
                    if not ally.IsEngaged() then
                        print("Engaging monster that is engaged");
                        allyGlobal.EngageTarget(ally, chara, nil);
                        break;
                    end
                end
            end
        end
    end
end

function allyGlobal.tryAggro(ally, contentGroupCharas)
    local count = 0;
    if contentGroupCharas and not ally.IsEngaged() then
        for i = 0, #contentGroupCharas - 1 do
            if contentGroupCharas[i] and ally then
                if contentGroupCharas[i].IsPlayer() then
                    -- probably a player, or another ally
                    -- todo: queue support actions, heal, try pull hate off player etc
                    if contentGroupCharas[i].target then
                        if ally.aiContainer:GetTargetFind():CanTarget(contentGroupCharas[i].target) and contentGroupCharas[i].target.IsMonster() and contentGroupCharas[i].target.hateContainer:HasHateForTarget(contentGroupCharas[i]) then
                            -- do stuff
                            allyGlobal.EngageTarget(ally, contentGroupCharas[i].target, nil);
                            break;
                        end
                    end
                elseif contentGroupCharas[i].IsMonster() and contentGroupCharas[i].IsEngaged() then
                    if not ally.IsEngaged() then
                        print("Engaging monster that is engaged");
                        allyGlobal.EngageTarget(ally, contentGroupCharas[i], nil);
                        break;
                    end
                end
            end
        end
    end
end

function allyGlobal.HealPlayer(ally, player)

end

function allyGlobal.SupportAction(ally, player)

end

function allyGlobal.EngageTarget(ally, target, contentGroupCharas)
    if contentGroupCharas then
        for chara in contentGroupCharas do
            if chara.IsMonster() then
                if chara.allegiance ~= ally.allegiance then
                    ally.Engage(chara)
                    break;
                end
            end
        end
    elseif target then
        print("Engaging");
        ally.Engage(target)
        ally.hateContainer.AddBaseHate(target);
    end
end