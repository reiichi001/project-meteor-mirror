require("/quests/man/man0g0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)
	
	man0g0Quest = player:getQuest("Man0g0");
	
	if (triggerName == "talkDefault") then		
		if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == false) then
			player:runEventFunction("delegateEvent", player, man0g0Quest, "processEvent000_2", nil, nil, nil);
			man0g0Quest:SetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1, true);				
			man0g0Quest:SaveData();			
			player:getDirector():onTalked(npc);	
		else
			player:runEventFunction("delegateEvent", player, man0g0Quest, "processEvent000_2", nil, nil, nil);
		end
	else
		player:endEvent();
	end
	
end

function onEventUpdate(player, npc)	
	player:endEvent();
end