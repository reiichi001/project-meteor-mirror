require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description = 
[[
Removes <keyitem> from player or <targetname>.
!delkeyitem <keyitem> |
!delkeyitem <keyitem> <target name> |
]],
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
        qty = tonumber(qty) or 1;
        local location = INVENTORY_KEYITEMS;
       
        local removed = player:GetItemPackage(location):RemoveItem(keyitem, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "Attempting to remove keyitem" -- "unable to remove keyitem";
        
        if removed then
            message = string.format("removed keyitem %u from %s", keyitem, player:GetName());
        end;
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to remove keyitem, ensure player name is valid.");
    end;
end;