require ("global")

function onEventStarted(player, npc)
	defaultSea = GetStaticActor("DftSea");
	callClientFunction(player, "delegateEvent", player, defaultSea, "
	player:RunEventFunction("delegateEvent", player, defaultSea, "defaultTalkWithH_lahono_001", nil, nil, nil);
	player:endEvent();
end
