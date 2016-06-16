properties = {
    permissions = 0,
    parameters = "sssss",
    description = [[changes appearance for equipment in <slot>. Parameters: <slot> <wId> <eId> <vId> <cId>,
                            idk what any of those mean either]],
}

function onTrigger(player, argc, slot, wId, eId, vId, cId)
    slot = tonumber(slot) or 0;
    wId = tonumber(wId) or 0;
    eId = tonumber(eId) or 0;
    vId = tonumber(vId) or 0;
    cId = tonumber(cId) or 0;
    
    if player then
        player:GraphicChange(slot, wId, eId, vId, cId);
        player:SendAppearance();
    end;
end;