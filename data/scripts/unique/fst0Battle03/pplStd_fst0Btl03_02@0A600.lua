require("/quests/man/man0g0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.GetActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc, triggerName)
	
	man0g0Quest = player:GetQuest("Man0g0");
	
	if (triggerName == "talkDefault") then		
		if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == false) then
			player:RunEventFunction("delegateEvent", player, man0g0Quest, "processEvent000_2", nil, nil, nil);
			man0g0Quest:SetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1, true);				
			man0g0Quest:SaveData();			
			player:GetDirector():OnTalked(npc);	
		else
			player:RunEventFunction("delegateEvent", player, man0g0Quest, "processEvent000_2", nil, nil, nil);
		end
	else
		player:EndEvent();
	end
	
end

function onEventUpdate(player, npc)	
	player:EndEvent();
end