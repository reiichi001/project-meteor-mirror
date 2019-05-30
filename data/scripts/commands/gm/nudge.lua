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

vertical = {
["UP"] = 1,
["U"] = 1,
["+"] = 1,
["ASCEND"] = 1,
["DOWN"] = -1,
["D"] = -1,
["-"] = -1,
["DESCEND"] = -1,
}

function onTrigger(player, argc, arg1, arg2)
    local pos = player:GetPos();
    local x = pos[0];
    local y = pos[1];
    local z = pos[2];
    local rot = pos[3];	
    local zone = pos[4];
    local angle = rot + (math.pi/2); 
    
    local worldManager = GetWorldManager();
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[nudge] ";   
 	local distance = 5;
    local direction = 0;

    local checkArg1 = tonumber(arg1);
    local checkArg2 = tonumber(arg2);
    
    if argc == 1 then
        if checkArg1 then
            distance = checkArg1;
        else
            player:SendMessage(messageID, sender, "Unknown parameters! Usage: \n"..properties.description);
            return;
        end
    elseif argc == 2 then
        if checkArg1 and checkArg2 then           -- If both are numbers, just ignore second argument
            distance = checkArg1;
        elseif checkArg1 and not checkArg2 then   -- If first is number and second is string
            distance = checkArg1;
            if vertical[string.upper(arg2)] then                -- Check vertical direction on string, otherwise throw param error
                direction = vertical[string.upper(arg2)];
            else
               player:SendMessage(messageID, sender, "Unknown parameters! Usage: \n"..properties.description);
               return;
            end
        elseif (not checkArg1) and checkArg2 then -- If first is string and second is number
            distance = checkArg2;
            if vertical[string.upper(arg1)] then                -- Check vertical direction on string, otherwise throw param error
                direction = vertical[string.upper(arg1)];
            else
                player:SendMessage(messageID, sender, "Unknown parameters! Usage: \n"..properties.description);
                return;
            end
        else
            player:SendMessage(messageID, sender, "Unknown parameters! Usage: \n"..properties.description);
            return;
        end
    end
    

    
    local message = string.format("Positioning forward %s yalms.", distance);

    if direction == 1 then
        y = y + distance;
        message = string.format("Positioning up %s yalms.", distance);
        worldManager:DoPlayerMoveInZone(player, x, y, z, rot, 0x0);
    elseif direction == -1 then
        y = y - distance;
        message = string.format("Positioning down %s yalms.", distance);
        worldManager:DoPlayerMoveInZone(player, x, y, z, rot, 0x0);
    else
        local px = x - distance * math.cos(angle);
        local pz = z + distance * math.sin(angle);
        if distance < 1 then
            message = string.format("Positioning back %s yalms.", distance);
        end
        worldManager:DoPlayerMoveInZone(player, px, y, pz, rot, 0x0);
    end;

    player:SendMessage(messageID, sender, message);	

end;
