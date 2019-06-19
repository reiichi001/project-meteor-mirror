properties = {
    permissions = 0,
    parameters = "ssss",
    description =
[[
Sends a custom <packet> to player or <targetname>
!sendpacket <packet> |
!sendpacket <packet> <targetname> |
]],
}

function onTrigger(player, argc, path, name, lastName)
    local sender = "[sendpacket ]";
    lastName = lastName or "";
    path = "./packets/"..path;
    
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    value = tonumber(value) or 0;
    if player and argc > 0 then
        player:SendPacket(path)
    end;
end;