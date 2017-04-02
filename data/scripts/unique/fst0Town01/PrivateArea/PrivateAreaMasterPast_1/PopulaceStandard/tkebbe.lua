require ("global")

function onSpawn(player, npc)	
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)
	man0g1Quest = player:GetQuest("Man0g0");
	
	if (man0g1Quest ~= nil) then	
		if (triggerName == "talkDefault") then
			callClientFunction(player, "delegateEvent", player, man0g1Quest, "processEvent020_5");
			npc:SetQuestGraphic(player, 0x0);
		end		
	end
	
	player:EndEvent();
	
end