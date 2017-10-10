require ("global")
require ("quests/man/man0l0")

function onSpawn(player, npc)

	man0l0Quest = player:GetQuest("Man0l0");
	
	if (man0l0Quest ~= nil) then
		if (man0l0Quest ~= nil and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE1) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE2) == true and man0l0Quest:GetQuestFlag(MAN0L0_FLAG_MINITUT_DONE3) == true) then
			player:SetEventStatus(npc, "pushDefault", true, 0x2);
			npc:SetQuestGraphic(player, 0x3);
		else
			player:SetEventStatus(npc, "pushDefault", true, 0x0);
			npc:SetQuestGraphic(player, 0x0);
		end
	end
	
end

function onEventStarted(player, npc, triggerName)
	man0l0Quest = player:GetQuest("Man0l0");
	choice = callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEventNewRectAsk", nil);	
	
	if (choice == 1) then
		callClientFunction(player, "delegateEvent", player, man0l0Quest, "processEvent000_2", nil, nil, nil, nil);
		player:EndEvent();
		
		man0l0Quest:NextPhase(5);
		
		contentArea = player:GetZone():CreateContentArea(player, "/Area/PrivateArea/Content/PrivateAreaMasterSimpleContent", "man0l01", "SimpleContent30002", "Quest/QuestDirectorMan0l001");
		
		if (contentArea == nil) then
			player:EndEvent();
			return;
		end
		
		director = contentArea:GetContentDirector();		
		player:AddDirector(director);		
		director:StartDirector(false);
		
		player:KickEvent(director, "noticeEvent", true);
		player:SetLoginDirector(director);		
		
		GetWorldManager():DoZoneChangeContent(player, contentArea, -5, 16.35, 6, 0.5, 16);		
		
	else
		player:EndEvent();
	end	
end