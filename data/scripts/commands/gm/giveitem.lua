require("global");

properties = {
    permissions = 0,
    parameters = "sssss",
    description = 
[[
Adds <item> <qty> to <location> for player or <targetname>.
!giveitem <item> <qty> |
!giveitem <item> <qty> <location> |
!giveitem <item> <qty> <location> <targetname> |
]],
}

function onTrigger(player, argc, item, qty, location, name, lastName)
    local sender = "[giveitem] ";
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local message = string.format("Unable to add item %u", item);
     
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
            
            if not location then
                player:SendMessage(messageID, sender, "Unknown item location.");
                return;
            end;                
        else
            location = INVENTORY_NORMAL;
        end;
        
        local added = player:getInventory(location):addItem(item, qty, 1);
        
        if added then
            message = string.format("Added item %u of kind %u to %s", item, location, player:GetName());
        end;        
    else
        print(sender.."[giveitem] Unable to add item, ensure player name is valid.");
        return;
    end;
    
    player:SendMessage(messageID, sender, message);
    print(message);
end;