require ("global")
require ("quests/etc/etc5g0")

function onSpawn(player, npc)
	
	if (player:HasQuest("Etc5g0") == true and player:GetQuest("Etc5g0"):GetPhase() == 0) then
		npc:SetQuestGraphic(player, 0x2);
	else
		npc:SetQuestGraphic(player, 0x0);
	end
	
end

function onEventStarted(player, npc)
    defaultFst = GetStaticActor("DftFst");
	quest = player:GetQuest("Etc5g0");
	
	result = 1;	
	if (player:HasQuest("Etc5g0")) then
		unknown, result = callClientFunction(player, "switchEvent", defaultFst, quest, nil, nil, 1, 1, 0x3f1);
	end
	
	if (result == 1) then		
		callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithPfarahr_001", -1, -1);
	elseif (result == 2) then
		
		ownedQuest = player:GetQuest("Etc5g0");
		if (ownedQuest:GetPhase() == 0) then				
			callClientFunction(player, "delegateEvent", player, quest, "processEvent_010");		
			worldMaster = GetWorldMaster();
			player:SendGameMessage(player, worldMaster, 25225, ownedQuest:GetQuestId());	
			player:SendDataPacket("attention", worldMaster, "", 25225, ownedQuest:GetQuestId());	
			ownedQuest:NextPhase(1);
			npc:SetQuestGraphic(player, 0x0);
			vkorolon = GetWorldManager():GetActorInWorldByUniqueId("vkorolon");
			if (vkorolon ~= nil) then
				vkorolon:SetQuestGraphic(player, 0x4);
			end
		else
			callClientFunction(player, "delegateEvent", player, quest, "processEvent_010_1");
		end
	
	end
	
	player:endEvent();
end