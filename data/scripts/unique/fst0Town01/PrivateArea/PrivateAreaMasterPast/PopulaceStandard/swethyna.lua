require ("global")

function onEventStarted(player, npc, triggerName)
	man0g1Quest = player:GetQuest("Man0g0");
	
	if (man0g1Quest ~= nil) then	
		if (triggerName == "talkDefault") then
			callClientFunction(player, "delegateEvent", player, man0g1Quest, "processEvent020_2");		
			
		end		
	end
	
	player:EndEvent();

end