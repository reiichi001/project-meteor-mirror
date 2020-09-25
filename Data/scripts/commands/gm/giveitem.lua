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
    local worldMaster = GetWorldMaster(); 
     
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        item = tonumber(item) or nil;
        
        if not item then
            player:SendMessage(messageID, sender, "Invalid parameter for item.");
            return;
        end                
        
        qty = tonumber(qty) or 1;
        
		if location then 
			location = _G[string.upper(location)];
            
            if not location then
                player:SendMessage(messageID, sender, "Unknown item location.");
                return;
            end;                
        else
            location = INVENTORY_NORMAL;
        end;
        
        local invCheck = player:getItemPackage(location):addItem(item, qty, 1);       
        
        if (invCheck == INV_ERROR_FULL) then
            -- Your inventory is full.
            player:SendGameMessage(player, worldMaster, 60022, messageID);
        elseif (invCheck == INV_ERROR_ALREADY_HAS_UNIQUE) then
            -- You cannot have more than one <itemId> <quality> in your possession at any given time.
            player:SendGameMessage(player, worldMaster, 40279, messageID, item, 1);
        elseif (invCheck == INV_ERROR_SYSTEM_ERROR) then
            player:SendMessage(MESSAGE_TYPE_SYSTEM, "", "[DEBUG] Server Error on adding item.");
        elseif (invCheck == INV_ERROR_SUCCESS) then
            message = string.format("Added item %s to location %s to %s", item, location, player:GetName());
            player:SendMessage(MESSAGE_TYPE_SYSTEM, "", message);
            player:SendGameMessage(player, worldMaster, 25246, messageID, item, qty);
        end
    end

end;