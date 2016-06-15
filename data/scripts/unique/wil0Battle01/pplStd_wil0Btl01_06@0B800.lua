require("/quests/man/man0u0")

function init(npc)
	return "/Chara/Npc/Populace/PopulaceStandard", false, false, false, false, false, npc:GetActorClassId(), false, false, 0, 1, "TEST";	
end

function onSpawn(player, npc)
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)

	man0u0Quest = player:GetQuest("man0u0");	

	if (triggerName == "talkDefault") then
		if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == false) then
			player:RunEventFunction("delegateEvent", player, man0u0Quest, "processTtrMini003_first", nil, nil, nil);
			npc:SetQuestGraphic(player, 0x0);
			man0u0Quest:SetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3, true);
			man0u0Quest:SaveData();		
			player:GetDirector():OnTalked(npc);
		else
			player:RunEventFunction("delegateEvent", player, man0u0Quest, "processTtrMini003", nil, nil, nil);
		end
	else		
		player:EndEvent();
	end		
	
end

function onEventUpdate(player, npc)	
	player:EndEvent();
end