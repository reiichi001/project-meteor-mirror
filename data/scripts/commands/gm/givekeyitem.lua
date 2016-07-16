require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description = "adds <keyitem> to self or <target>.",
}

function onTrigger(player, argc, keyitem, name, lastName)
    local sender = "[givekeyitem] ";
    
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
        
        local added = player:GetInventory(location):AddItem(keyitem, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "unable to add keyitem";
        
        if keyitem and added then
            message = string.format("added keyitem %u to %s", keyitem, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to add keyitem, ensure player name is valid.");
    end;
end;