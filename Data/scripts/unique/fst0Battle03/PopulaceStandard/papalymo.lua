require ("global")
require ("quests/man/man0g0")

function onEventStarted(player, npc, triggerName)	
	man0g0Quest = player:GetQuest("Man0g0");
	
	if (triggerName == "talkDefault") then		
		if (man0g0Quest:GetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1) == false) then
			callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent000_3", nil, nil, nil);
			man0g0Quest:SetQuestFlag(MAN0G0_FLAG_MINITUT_DONE1, true);				
			man0g0Quest:SaveData();			
			npc:SetQuestGraphic(player, 0x0);
			player:GetDirector("OpeningDirector"):onTalkEvent(player, npc);	
		else
			callClientFunction(player, "delegateEvent", player, man0g0Quest, "processEvent000_2", nil, nil, nil);
		end
	end	
	player:EndEvent();	
end