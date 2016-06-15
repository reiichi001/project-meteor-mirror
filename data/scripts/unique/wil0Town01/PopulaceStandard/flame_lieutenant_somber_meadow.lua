
function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
    player:RunEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithFlamelieutenantsombermeadow_001", nil, nil, nil);
end

