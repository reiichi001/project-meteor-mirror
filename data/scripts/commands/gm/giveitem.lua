require("global");

properties = {
    permissions = 0,
    parameters = "sssss",
    description = "adds <item> <qty> to <location> for <target>. <qty> and <location> are optional, item is added to user if <target> is nil",
}

function onTrigger(player, argc, item, qty, location, name, lastName)
    local sender = "[giveitem] ";
    
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
        local added = player:GetInventory(location):AddItem(item, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "unable to add item";
        
        if item and added then
            message = string.format("added item %u to %s", item, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to add item, ensure player name is valid.");
    end;
end;