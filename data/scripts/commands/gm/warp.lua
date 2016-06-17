require("global");

properties = {
    permissions = 0,
    parameters = "sssssss",
    description =
[[
<zone> |
<zone> <x> <y> <z> |
<x> <y> <z> <zone> <privateArea> <target name>.
]],
}

function onTrigger(player, argc, p1, p2, p3, p4, privateArea, name, lastName)

    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if not player then
        printf("[Command] [warp] error! No target or player specified!");
        return;
    end;
    
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[warp] ";
    
    -- we're getting a list/array from c# so 0 index
    local pos = player:GetPos();
    local player_x = pos[0];
    local player_y = pos[1];
    local player_z = pos[2];
    local player_rot = pos[3];
    local player_zone = pos[4];
    
    local worldManager = GetWorldManager();
    
    -- treat this as a predefined warp list
    if argc == 1 then
        zone = tonumber(p1) or player_zone;
        player:SendMessage(messageID, sender,  string.format("warping to zone:%u", zone));
        worldManager:DoZoneChange(player, zone);
    
    elseif argc >= 3 then
        
        if argc == 3 then
            local x = tonumber(applyPositionOffset(p1, player_x)) or player_x;
            local y = tonumber(applyPositionOffset(p2, player_y)) or player_y;
            local z = tonumber(applyPositionOffset(p3, player_z)) or player_z;
            
            player:SendMessage(messageID, sender, string.format("setting coordinates X:%d Y:%d Z:%d within current zone (%d)", x, y, z, player_zone));
            worldManager:DoPlayerMoveInZone(player, x, y, z, 0x0F);
        else
            local zone = tonumber(applyPositionOffset(p1, player_zone)) or player_zone;
            local x = tonumber(applyPositionOffset(p2, player_x)) or player_x;
            local y = tonumber(applyPositionOffset(p3, player_y)) or player_y;
            local z = tonumber(applyPositionOffset(p4, player_z)) or player_z;
            if privateArea == "" then privateArea = nil end;
            player:SendMessage(messageID, sender, string.format("setting coordinates X:%d Y:%d Z:%d to new zone (%d) private area:%s", x, y, z, zone, privateArea or "unspecified"));
            worldManager:DoZoneChange(player, zone, privateArea, 0x02, x, y, z, 0.00);
        end
  
    else
        player:SendMessage(messageID, sender, "Unknown parameters! Usage: "..properties.description);
    end;
end;

function applyPositionOffset(str, offset)
    local s = str;
    print(s);
    if s:find("@") then
        s = tonumber(s:sub(s:find("@") + 1, s:len())) + offset;
    end
    return s;
end;