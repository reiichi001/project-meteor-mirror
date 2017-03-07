require ("global")
require ("quests/man/man0u0")

function onSpawn(player, npc)
	man0u0Quest = player:GetQuest("Man0u0");

	if (man0u0Quest ~= nil) then
		if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == false) then
			npc:SetQuestGraphic(player, 0x2);
		end
	end	
end

function onEventStarted(player, npc, triggerName)
	man0u0Quest = player:GetQuest("Man0u0");

	if (man0u0Quest ~= nil) then
		if (triggerName == "talkDefault") then
			if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == false) then
				callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrMini003_first", nil, nil, nil);
				npc:SetQuestGraphic(player, 0x0);
				man0u0Quest:SetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3, true);
				man0u0Quest:SaveData();				
				player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);
			else
				callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrMini003", nil, nil, nil);
			end
		end
	end
	player:EndEvent();
end