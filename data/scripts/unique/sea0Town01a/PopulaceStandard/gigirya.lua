function onEventStarted(player, npc)
	defaultSea = getStaticActor("DftSea");
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGigirya_001", nil, nil, nil);
	--player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGigirya_002", nil, nil, nil);     --THM
	player:runEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGigirya_003", nil, nil, nil);   --THM NO GUILD
end

function onEventUpdate(player, npc)
	player:endEvent();
end