require("global");

properties = {
    permissions = 0,
    parameters = "sssss",
    description = 
[[
Removes <item> <qty> from <location> for player or <targetname>.
!delitem <item> <qty> |
!delitem <item> <qty> <location> |
!delitem <item> <qty> <location> <targetname> |
]],
}

function onTrigger(player, argc, item, qty, location, name, lastName)
    local sender = "[delitem] ";
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    
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
        
		if location then 
			location =  tonumber(location) or _G[string.upper(location)];
            
            if location == nil then
                player:SendMessage(messageID, sender, "Unknown item location.");
                return;
            end;                
        else
            location = INVENTORY_NORMAL;
        end;
        
        local removed = player:GetItemPackage(location):RemoveItem(item, qty);
        
        if removed then  -- RemoveItem() currently returns nothing for verification, this statement can't work
            message = string.format("Removed item %u of kind %u to %s", item, location, player:GetName());
        end;        
    else
        print(sender.."[giveitem] Unable to remove item, ensure player name is valid.");
        return;
    end;
    
    local message = string.format("Attempting to remove item %u of kind %u from %s", item, location, player:GetName());
    player:SendMessage(messageID, sender, message);
    print(message);
end;