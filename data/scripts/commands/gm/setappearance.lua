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

function onTrigger(player, argc, appearanceId)
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[setappearance] ";
    
    app = tonumber(appearanceId) or 0;
    player:SendMessage(messageID, sender,  string.format("appearance %u", app));
    
    if player and player.target then
        player.target.ChangeNpcAppearance(app);
        player:SendMessage(messageID, sender,  string.format("appearance %u", app));
    end;
           
end;