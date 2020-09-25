require ("global")
require ("quests/man/man0u0")

function onEventStarted(player, npc, triggerName)		
	man0u0Quest = GetStaticActor("Man0u0");	
	
	if (man0u0Quest ~= nil) then		
		callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrBlkNml002", nil, nil, nil);
		GetWorldManager():DoZoneChange(player, 175, "PrivateAreaMasterPast", 3, 15, -22.81, 196, 87.82, 2.98);
	end
	
	player:EndEvent();	
end