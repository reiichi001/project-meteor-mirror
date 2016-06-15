
function onEventStarted(player, npc)
    defaultWil = GetStaticActor("DftWil");
    player:RunEventFunction("delegateEvent", player, defaultWil, "tribeTalk", nil, nil, nil);
end

