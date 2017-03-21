require ("global")
require ("quests/etc/etc3g0")

function onSpawn(player, npc)
	
	if (player:HasQuest("Etc3g0") == false and player:GetQuest("Etc3g0"):GetQuestFlag(TALKED_4)) then
		npc:SetQuestGraphic(player, 0x2);
	else
		npc:SetQuestGraphic(player, 0x0);
	end
	
end

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("DftFst");
	quest = GetStaticActor("Etc3g0");
	
	unknown, result = callClientFunction(player, "switchEvent", defaultFst, quest, nil, nil, 1, 1, 0x3f1);
	
	if (result == 1) then		
		callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithKinnison_001", -1, -1);
	elseif (result == 2) then
		if (player:HasQuest("Etc3g0") == false) then
			offerQuestResult = callClientFunction(player, "delegateEvent", player, quest, "processEventOffersStart");
			if (offerQuestResult == 1) then
				player:AddQuest("Etc3g0");
				npc:SetQuestGraphic(player, 0x0);
			end
		else
			ownedQuest = player:GetQuest("Etc3g0");
			if (ownedQuest:GetPhase() == 1) then				
				callClientFunction(player, "delegateEvent", player, quest, "processEventClear");
				callClientFunction(player, "delegateEvent", player, quest, "sqrwa", 200, 1, 1, 9);
				player:CompleteQuest("Etc3g0");
			else
				callClientFunction(player, "delegateEvent", player, quest, "processEventOffersAfter");
			end
		end
	end
	
	player:endEvent();
end