require ("global")

function onEventStarted(player, npc)
    Spl = GetStaticActor("Spl000");
    magickedPrism = 3020615;

    if not player:GetItemPackage(INVENTORY_NORMAL):HasItem(magickedPrism) then    
        callClientFunction(player, "delegateEvent", player, Spl, "processEventELNAURE", 2);
        local invCheck = player:GetItemPackage(INVENTORY_NORMAL):AddItem(magickedPrism, 10, 1);
        if invCheck == INV_ERROR_SUCCESS then
            player:SendGameMessage(player, GetWorldMaster(), 25246, MESSAGE_TYPE_SYSTEM, magickedPrism, 10);
        end
    else
        callClientFunction(player, "delegateEvent", player, Spl, "processEventELNAURE", 1);
    end
    
    player:endEvent();
end