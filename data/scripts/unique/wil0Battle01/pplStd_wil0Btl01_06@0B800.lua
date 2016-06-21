require ("global")
require ("quests/man/man0u0")

function onEventStarted(player, npc, triggerName)
	man0u0Quest = GetStaticActor("Man0u0");	
	
	if (man0u0Quest:GetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3) == false) then
			callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrMini003_first", nil, nil, nil);
			npc:SetQuestGraphic(player, 0x0);
			man0u0Quest:SetQuestFlag(MAN0U0_FLAG_MINITUT_DONE3, true);
			man0u0Quest:SaveData();		
			player:GetDirector():OnTalked(npc);
		else
			callClientFunction(player, "delegateEvent", player, man0u0Quest, "processTtrMini003", nil, nil, nil);
		end
		
	player:EndEvent();
end