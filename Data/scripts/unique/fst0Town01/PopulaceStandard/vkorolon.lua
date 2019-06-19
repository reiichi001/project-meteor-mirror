require ("global")
require ("quests/etc/etc5g0")

function onSpawn(player, npc)
	
	if (player:HasQuest("Etc5g0") == true and player:GetQuest("Etc5g0"):GetPhase() == 1) then
		npc:SetQuestGraphic(player, 0x4);
	elseif (canAcceptQuest(player)) then
		npc:SetQuestGraphic(player, 0x2);
	else
		npc:SetQuestGraphic(player, 0x0);
	end
	
end

function onEventStarted(player, npc)
	defaultFst = GetStaticActor("DftFst");
	quest = GetStaticActor("Etc5g0");
	
	result = 1;	
	
	if (player:IsQuestCompleted("Etc5g0") == true) then
		result = 0;
	else
		unknown, result = callClientFunction(player, "switchEvent", defaultFst, quest, nil, nil, 1, 1, 0x3f1);
	end		
	
	if (result == 0) then
		choice = callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithInn_Desk", nil, nil, nil);
		
		if (choice == 1) then
			GetWorldManager():DoZoneChange(player, 244, nil, 0, 15, 160.048, 0, 154.263, 0);
		elseif (choice == 2) then			
			if (player:GetHomePointInn() ~= 2) then
				player:SetHomePointInn(2);
				player:SendGameMessage(GetWorldMaster(), 60019, 0x20, 2075); --Secondary homepoint set to the Roost
			else			
				player:SendGameMessage(GetWorldMaster(), 51140, 0x20); --This inn is already your Secondary Homepoint
			end
		end
	elseif (result == 1) then		
		callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithVkorolon_001", -1, -1);
	elseif (result == 2) then		
		if (player:HasQuest("Etc5g0") == false) then
			offerQuestResult = callClientFunction(player, "delegateEvent", player, quest, "processEventVKOROLONStart");
			if (offerQuestResult == 1) then
				player:AddQuest("Etc5g0");
				npc:SetQuestGraphic(player, 0x0);
				pfarahr = GetWorldManager():GetActorInWorldByUniqueId("pfarahr");
				if (pfarahr ~= nil) then
					pfarahr:SetQuestGraphic(player, 0x2);
				end
			end
		elseif (player:GetQuest("Etc5g0"):GetPhase() == 0) then
			callClientFunction(player, "delegateEvent", player, quest, "processEvent_000_1");
		elseif (player:GetQuest("Etc5g0"):GetPhase() == 1) then
			callClientFunction(player, "delegateEvent", player, quest, "processEvent_020");
			callClientFunction(player, "delegateEvent", player, quest, "sqrwa", 200, 1);
			player:CompleteQuest("Etc5g0");
			npc:SetQuestGraphic(player, 0x0);
		end		
	
	end	
	
	player:EndEvent();
	
end