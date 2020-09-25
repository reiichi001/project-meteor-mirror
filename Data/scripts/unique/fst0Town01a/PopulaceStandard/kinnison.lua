require ("global")
require ("quests/etc/etc3g0")

function onSpawn(player, npc)
	
	if (player:HasQuest("Etc3g0") == true and player:GetQuest("Etc3g0"):GetPhase() == 243) then
		npc:SetQuestGraphic(player, 0x4);
	elseif (canAcceptQuest(player)) then
		npc:SetQuestGraphic(player, 0x2);
	else
		npc:SetQuestGraphic(player, 0x0);
	end	
	
end

function onEventStarted(player, npc)

    defaultFst = GetStaticActor("DftFst");
	quest = GetStaticActor("Etc3g0");
	
	if ((canAcceptQuest(player) == true) or (player:HasQuest("Etc3g0") == true)) then
	
		unknown, result = callClientFunction(player, "switchEvent", defaultFst, quest, nil, nil, 1, 1, 0x3f1);
	
		if (result == 1) then		
			callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithKinnison_001", -1, -1);
		elseif (result == 2) then
			if (player:HasQuest("Etc3g0") == false) then
				offerQuestResult = callClientFunction(player, "delegateEvent", player, quest, "processEventOffersStart");
				if (offerQuestResult == 1) then
					player:AddQuest("Etc3g0");
					npc:SetQuestGraphic(player, 0x0);
					
					-- This is to overcome some weirdness where some NPCs are not updating their quest marker upon quest accepted
					-- So we're just going to force the change to be sure
					mestonnaux = GetWorldManager():GetActorInWorldByUniqueId("mestonnaux");
					sybell = GetWorldManager():GetActorInWorldByUniqueId("sybell");
					khuma_moshroca = GetWorldManager():GetActorInWorldByUniqueId("khuma_moshroca");
					lefwyne = GetWorldManager():GetActorInWorldByUniqueId("lefwyne");
					nellaure = GetWorldManager():GetActorInWorldByUniqueId("nellaure");	
					
					if (mestonnaux ~= nil) then mestonnaux:SetQuestGraphic(player, 0x2); end
					if (sybell ~= nil) then sybell:SetQuestGraphic(player, 0x2); end
					if (khuma_moshroca ~= nil) then khuma_moshroca:SetQuestGraphic(player, 0x2); end
					if (lefwyne ~= nil) then lefwyne:SetQuestGraphic(player, 0x2); end
					if (nellaure ~= nil) then nellaure:SetQuestGraphic(player, 0x2); end
				
				end
			else
				ownedQuest = player:GetQuest("Etc3g0");
				if (ownedQuest:GetPhase() == 243) then				
					callClientFunction(player, "delegateEvent", player, quest, "processEventClear");
					callClientFunction(player, "delegateEvent", player, quest, "sqrwa", 200, 1, 1, 9);
					player:CompleteQuest("Etc3g0");
					npc:SetQuestGraphic(player, 0x0);
				else
					callClientFunction(player, "delegateEvent", player, quest, "processEventOffersAfter");
				end
			end
		end
	else
		callClientFunction(player, "delegateEvent", player, defaultFst, "defaultTalkWithKinnison_001", -1, -1);
	end	
	
	player:endEvent();
end