require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description = "removes <keyitem> <qty> from <target>, keyitem is removed from user if <target> is nil",
}

function onTrigger(player, argc, keyitem, qty, name, lastName)
    local sender = "[delkeyitem] ";
    
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        keyitem = tonumber(keyitem) or nil;
        qty = 1;
        location = INVENTORY_KEYITEMS;
        
        local removed = player:GetInventory(location):removeItem(item, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "unable to remove keyitem";
        
        if keyitem and removed then
            message = string.format("removed keyitem %u from %s", keyitem, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to remove keyitem, ensure player name is valid.");
    end;
end;