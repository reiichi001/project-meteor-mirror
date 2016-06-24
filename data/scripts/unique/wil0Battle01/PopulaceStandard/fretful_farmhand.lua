require ("global")
require ("quests/man/man0u0")

function onEventStarted(player, npc, triggerName)
	man0u0Quest = player:GetQuest("Man0u0");
	
	if (man0u0Quest != nil)	
		if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2) == false) then
			callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrMini002_first", nil, nil, nil);
			npc:SetQuestGraphic(player, 0x0);
			man0u0Quest:SetQuestFlag(MAN0U0_FLAG_MINITUT_DONE2, true);
			man0u0Quest:SaveData();		
			player:GetDirector():OnTalked(npc);
		else
			callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrMini002", nil, nil, nil);
		end
	end
	
	player:EndEvent();
end