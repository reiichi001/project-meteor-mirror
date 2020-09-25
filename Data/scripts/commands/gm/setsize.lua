require("global");

properties = {
    permissions = 0,
    parameters = "s",
    description =
[[
Changes appearance for equipment with given parameters.
!graphic <slot> <wID> <eID> <vID> <vID>
]],
}

function onTrigger(player, argc, size)
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[setappearance] ";
    
    s = tonumber(size) or 0;
    
    if player and player.target then
        player.target.appearanceIds[0] = s;
        player.target.zone.BroadcastPacketAroundActor(player.target, player.target.CreateAppearancePacket());
    end;
           
end;