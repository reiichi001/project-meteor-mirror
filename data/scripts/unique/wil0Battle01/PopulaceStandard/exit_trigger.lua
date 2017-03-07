require ("global")
require ("quests/man/man0u0")

function onSpawn(player, npc)

	man0u0Quest = player:GetQuest("Man0u0");
	
	if (man0u0Quest ~= nil) then
		player:SetEventStatus(npc, "pushDefault", true, 0x2);
		if (man0u0Quest ~= nil and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == true and man0U0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == true) then			
			npc:SetQuestGraphic(player, 0x3);
		else			
			npc:SetQuestGraphic(player, 0x0);
		end
	end
	
end

function onEventStarted(player, npc, triggerName)		
	man0u0Quest = GetStaticActor("Man0u0");	
	
	if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) ~= true) then
		print "AAAA"
	end
	
	--if (man0u0Quest ~= nil and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE1) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2) == true and man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == true) then
		
		player:EndEvent();
		
		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34108, 0x20);	
		player:SendGameMessage(player, worldMaster, 50011, 0x20);	

		director = player:GetZone():CreateDirector("Quest/QuestDirectorMan0u001");
		player:KickEvent(director, "noticeEvent", true);
		player:AddDirector(director);
		player:SetLoginDirector(director);
		
		GetWorldManager():DoZoneChange(player, 17);
	
	
	
end

--[[AFTER GOOBBUE
22.81, 196, 87.82
]]
--0x45c00005