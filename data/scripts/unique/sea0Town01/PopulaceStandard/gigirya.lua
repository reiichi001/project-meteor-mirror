
function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGigirya_001", nil, nil, nil);
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGigirya_002", nil, nil, nil);     --THM
	--player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithGigirya_003", nil, nil, nil);   --THM NO GUILD
end