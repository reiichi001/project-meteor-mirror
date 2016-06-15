
function onEventStarted(player, npc)
    defaultWil = getStaticActor("DftWil");
    player:runEventFunction("delegateEvent", player, defaultWil, "tribeTalk", nil, nil, nil);
end

