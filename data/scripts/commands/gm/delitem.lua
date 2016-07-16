require("global");

properties = {
    permissions = 0,
    parameters = "sssss",
    description = "removes <item> <qty> from <location> for <target>. <qty> and <location> are optional, item is removed from user if <target> is nil",
}

function onTrigger(player, argc, item, qty, location, name, lastName)
    local sender = "[delitem] ";
    
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        item = tonumber(item) or nil;
        qty = tonumber(qty) or 1;
        location = tonumber(itemtype) or INVENTORY_NORMAL;
        
        local removed = player:GetInventory(location):removeItem(item, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "unable to remove item";
        
        if item and removed then
            message = string.format("removed item %u from %s", item, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to remove item, ensure player name is valid.");
    end;
end;