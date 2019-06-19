require ("global")
require ("quests/etc/etc3g0")

function onSpawn(player, npc)

   if (player:HasQuest("Etc3g0") == true and player:GetQuest("Etc3g0"):GetPhase() == 0) then	
		if player:GetQuest("Etc3g0"):GetQuestFlag(FLAG_TALKED_SYBELL) == false then
			npc:SetQuestGraphic(player, 0x2);
		else
			npc:SetQuestGraphic(player, 0x0);
		end
    else
        npc:SetQuestGraphic(player, 0x0);
    end
	
end



function onEventStarted(player, npc)

	defaultFst = GetStaticActor("DftFst");
    quest = GetStaticActor("Etc3g0");

	if (player:HasQuest("Etc3g0") == true) then
	
		unknown, result = callClientFunction(player, "switchEvent", defaultFst, quest, nil, nil, 1, 1, 0x3f1);
    
		if (result == 1) then        
			callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithSybell_001", nil, nil, nil);
		elseif (result == 2) then
			ownedQuest = player:GetQuest("Etc3g0");
			
			if (ownedQuest:GetQuestFlag(FLAG_TALKED_SYBELL)) == false then
				callClientFunction(player, "delegateEvent", player, quest, "processEventSybellSpeak", nil, nil, nil);
				ownedQuest:SetQuestFlag(FLAG_TALKED_SYBELL, true);
				ownedQuest:SaveData();	
				npc:SetQuestGraphic(player, 0x0);
				checkNextPhase(player);
			else
				callClientFunction(player, "delegateEvent", player, quest, "processEventSybellSpeakAfter", nil, nil, nil);
			end	
		end
		
	else
		callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithSybell_001", nil, nil, nil);
	end
	
	
	player:endEvent();
end



