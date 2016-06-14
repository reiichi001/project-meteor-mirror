
function onEventStarted(player, npc)
    defaultWil = getStaticActor("DftWil");
    player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithFlamelieutenantsombermeadow_001", nil, nil, nil);
end

