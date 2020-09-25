require ("global")

function onEventStarted(player, npc, triggerName)
	man0g0Quest = player:GetQuest("Man0g0");
		
	if (man0g0Quest ~= nil) then	
		if (triggerName == "talkDefault") then
			callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent020_6");
		end		
	end

	player:EndEvent();
	
end