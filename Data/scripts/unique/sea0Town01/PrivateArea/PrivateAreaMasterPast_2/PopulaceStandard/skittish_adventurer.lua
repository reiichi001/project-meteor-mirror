require ("global")

function onEventStarted(player, npc, triggerName)
	man0l1Quest = player:GetQuest("Man0l1");
	
	if (man0l1Quest ~= nil) then	
		if (triggerName == "talkDefault") then
			callClientFunction(player, "delegateEvent", player, man0l1Quest, "processEvent010_3");		
			player:EndEvent();
		end		
	end

end