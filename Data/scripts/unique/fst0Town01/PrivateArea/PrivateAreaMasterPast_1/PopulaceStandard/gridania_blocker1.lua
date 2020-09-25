require ("global")

function init(player, npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	man0g0Quest = player:GetQuest("Man0g0");
	if (man0g0Quest ~= nil) then			
		callClientFunction(player, "delegateEvent", player, man0g0Quest, "processTtrBlkNml001", nil, nil, nil);
		GetWorldManager():DoZoneChange(player, 155, "PrivateAreaMasterPast", 1, 15, 109.966, 7.559, -1206.117, -2.7916);	
	end
	player:EndEvent();
end