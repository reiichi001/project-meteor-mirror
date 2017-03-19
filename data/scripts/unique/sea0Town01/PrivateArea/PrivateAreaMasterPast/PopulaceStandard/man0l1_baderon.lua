require ("global")

function onSpawn(player, npc)
	man0l1Quest = player:GetQuest("Man0l1");	
	
	if (man0l1Quest ~= nil) then		
		npc:SetQuestGraphic(player, 0x2);
	end
end

function onEventStarted(player, npc, triggerName)
	man0l1Quest = player:GetQuest("Man0l1");
	
	if (man0l1Quest ~= nil) then	
		if (triggerName == "talkDefault") then
			callClientFunction(player, "delegateEvent", player, man0l1Quest, "processEvent020");		
			man0l1Quest:NextPhase(3);
			player:EndEvent();
			GetWorldManager():DoZoneChange(player, 133, nil, 0, 15, player.positionX, player.positionY, player.positionZ, player.rotation);
		end		
	end

end