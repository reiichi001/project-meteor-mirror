require("/quests/man/man0u0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)
	npc:setQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)

	man0u0Quest = player:getQuest("Man0u0");
	if (man0u0Quest ~= nil) then	
	
		if (triggerName == "pushDefault") then
			player:runEventFunction("delegateEvent", player, man0u0Quest, "processTtrNomal002", nil, nil, nil);			
		elseif (triggerName == "talkDefault") then		
			if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_TUTORIAL1_DONE) == false) then
				player:runEventFunction("delegateEvent", player, man0u0Quest, "processTtrNomal003", nil, nil, nil);			
				player:setEventStatus(npc, "pushDefault", false, 0x2);
				player:getDirector():onTalked(npc);			
				man0u0Quest:SetQuestFlag(MAN0U0_FLAG_TUTORIAL1_DONE, true);				
				man0u0Quest:SaveData();
			else
				player:runEventFunction("delegateEvent", player, man0u0Quest, "processTtrMini001", nil, nil, nil);
				
				if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == false) then
					npc:setQuestGraphic(player, 0x0);
					man0u0Quest:SetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1, true);
					man0u0Quest:SaveData();					
				end
				
			end
		else
			player:endEvent();
		end
	else
		player:endEvent(); --Should not be here w.o this quest
	end	
	
end

function onEventUpdate(player, npc)	
	
	player:endEvent();	
	
end