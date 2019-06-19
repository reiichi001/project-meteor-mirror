require("global");

properties = {
    permissions = 0,
    parameters = "ssss",
    description =
[[
Warps to name of player, or warps first player to second player
<target name> |
<1st target name> <2nd target name>
]],
}

function onTrigger(player, argc, name, lastName, name2, lastName2)
    
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[warpplayer] ";

    if name and lastName then
        p1 = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
    end;  
    
    if name2 and lastName2 then
        p2 = GetWorldManager():GetPCInWorld(name2.." "..lastName2) or nil;
    end;
    
    if not player then
        printf("[Command] [warpplayer] Error! No target or player specified!");
        return;
    end;
    
    local worldManager = GetWorldManager();
        
    if argc == 2 then
        if not p1 then
            printf("[Command] [warpplayer] Error! Invalid player specified!");
            player:SendMessage(messageID, sender, "Error! Invalid player specified!");
            return;    
        else
            local pos = p1:GetPos();
            worldManager:DoZoneChange(player, pos[4], nil, 0, 0x02, pos[0], pos[1], pos[2], pos[3]);
            player:SendMessage(messageID, sender, string.format("Moving to %s %s 's coordinates.", name, lastName));
        end;
    elseif argc == 4 then;
        if not p1 or not p2 then
            printf("[Command] [warpplayer] Error! Invalid player specified!");
            player:SendMessage(messageID, sender, "Error! Invalid player specified!");
            return;
        else
            local pos = p1:GetPos();
            local pos2 = p2:GetPos();
            
            worldManager:DoZoneChange(p1, pos2[4], nil, 0, 0x02, pos2[0], pos2[1], pos2[2], pos2[3]);
            player:SendMessage(messageID, sender, string.format("Moving %s %s to %s %s 's coordinates.", name, lastName, name2, lastName2));    
            p1:SendMessage(messageID, sender, string.format("You are being moved to %s %s 's coordinates.", name2, lastName2));              
        end;
    else
        if player then
            player:SendMessage(messageID, sender, "Unknown parameters! Usage: "..properties.description);
        end;
        return;
    end;
end;
