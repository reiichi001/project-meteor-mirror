require("global");

properties = {
    permissions = 0,
    parameters = "sssss",
    description =
[[
Changes appearance for equipment with given parameters.
!graphic <slot> <wID> <eID> <vID> <vID>
]],
}

function onTrigger(player, argc, slot, wId, eId, vId, cId)
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "[graphic] ";
    
    slot = tonumber(slot) or 0;
    wId = tonumber(wId) or 0;
    eId = tonumber(eId) or 0;
    vId = tonumber(vId) or 0;
    cId = tonumber(cId) or 0;
    
    if player and argc > 0 then
        if argc > 2 then
            player:GraphicChange(slot, wId, eId, vId, cId);
            player:SendMessage(messageID, sender,  string.format("Changing appearance on slot %u", slot));
        else
            player:GraphicChange(slot, wId);
        end
        player:SendAppearance();
    else
        player:SendMessage(messageID, sender, "No parameters sent! Usage: "..properties.description);
    end;
           
end;