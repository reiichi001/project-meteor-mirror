
function onEventStarted(player, npc)
    defaultWil = getStaticActor("DftWil");
    player:runEventFunction("delegateEvent", player, defaultWil, "defaultTalkWithMaginfred_001", nil, nil, nil);
end

