require("global");

properties = {
    permissions = 0,
    parameters = "ss",
    description =
[[
Positions your character forward a set <distance>, defaults to 5 yalms.
!nudge |
!nudge <distance> |
!nudge <distance> <up/down> |
]],

}

function onTrigger(player, argc, distance, vertical)
    local pos = player:GetPos();
    local x = pos[0];
    local y = pos[1];
    local z = pos[2];
    local rot = pos[3];	
    local zone = pos[4];
    local angle = rot + (math.pi/2); 
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[nudge] ";   
 	
 	if distance == nil then
	    distance = 5
    end;
    
    local px = x - distance * math.cos(angle);
    local pz = z + distance * math.sin(angle);
    local message = string.format("Positioning forward %u yalms.", distance);
    local worldManager = GetWorldManager();
    
    if argc == 1 then
    	worldManager:DoPlayerMoveInZone(player, px, y, pz, rot, 0x0);
    	player:SendMessage(messageID, sender, message);
  	elseif argc == 2 then
    	if vertical == "up" or vertical == "u" or vertical == "+" then
     		y = y + distance;
   			message = string.format("Positioning up %u yalms.", distance);
    		worldManager:DoPlayerMoveInZone(player, x, y, z, rot, 0x0);
    		player:SendMessage(messageID, sender, message);
   		elseif vertical == "down" or vertical == "d" or vertical == "-" then
   			y = y - distance;
   			message = string.format("Positioning down %u yalms.", distance);
    		worldManager:DoPlayerMoveInZone(player, x, y, z, rot, 0x0);
    		player:SendMessage(messageID, sender, message);
    	else
    		player:SendMessage(messageID, sender, "Unknown parameters! Usage: \n"..properties.description);
   		end;
    else
    	worldManager:DoPlayerMoveInZone(player, px, y, pz, rot, 0x0);
    	player:SendMessage(messageID, sender, message);	
    end;
end;
