require("/quests/man/man0u0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)
	npc:setQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)

	man0u0Quest = player:getQuest("man0u0");	

	if (triggerName == "talkDefault") then
		if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2) == false) then
			player:runEventFunction("delegateEvent", player, man0u0Quest, "processTtrMini002_first", nil, nil, nil);
			npc:setQuestGraphic(player, 0x0);
			man0u0Quest:SetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2, true);
			man0u0Quest:SaveData();		
			player:getDirector():onTalked(npc);
		else
			player:runEventFunction("delegateEvent", player, man0u0Quest, "processTtrMini002", nil, nil, nil);
		end
	else		
		player:endEvent();
	end		
	
end

function onEventUpdate(player, npc)	
	player:endEvent();
end