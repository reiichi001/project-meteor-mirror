function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)

	saySheetId = 1;

	if (npc:GetActorClassId() == 1000157) then
		saySheetId = 9;
	elseif (npc:GetActorClassId() == 1000158) then
		saySheetId = 24;
	elseif (npc:GetActorClassId() == 1000162) then
		saySheetId = 18;
	elseif (npc:GetActorClassId() == 1000164) then
		saySheetId = 16;
	elseif (npc:GetActorClassId() == 1000459) then
		saySheetId = 21;		
	elseif (npc:GetActorClassId() == 1000460) then
		saySheetId = 13;
	elseif (npc:GetActorClassId() == 1000461) then
		saySheetId = 15;
	elseif (npc:GetActorClassId() == 1000462) then
		saySheetId = 11;
	elseif (npc:GetActorClassId() == 1000464) then
		saySheetId = 10;
	elseif (npc:GetActorClassId() == 1000466) then
		saySheetId = 17;
	elseif (npc:GetActorClassId() == 1000631) then
		saySheetId = 8;
	elseif (npc:GetActorClassId() == 1000632) then
		saySheetId = 7;
	elseif (npc:GetActorClassId() == 1000633) then
		saySheetId = 12;
	elseif (npc:GetActorClassId() == 1000634) then
		saySheetId = 23;
	elseif (npc:GetActorClassId() == 1000635) then
		saySheetId = 20;
	elseif (npc:GetActorClassId() == 1000636) then
		saySheetId = 22;
	elseif (npc:GetActorClassId() == 1000637) then
		saySheetId = 14;
	elseif (npc:GetActorClassId() == 1001461) then
		saySheetId = 19;
	end

	player:RunEventFunction("welcomeTalk", nil, saySheetId, player);
end

function onEventUpdate(player, npc, step, menuOptionSelected)
	
	--player:RunEventFunction("cashbackTalkCommand", 22004, 22004, 22004, 22004, 22004, 22004, 22004, 22004, 22004, 22004, 22004); --Refund Abilities???
	--player:RunEventFunction("cashbackTalk", 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1); --Refund items???
	
	if (menuOptionSelected == 4) then
		player:EndEvent();
	else
		player:RunEventFunction("selectMode", nil, npc:GetActorClassId(), false, 1000001); --Step 2, state your business
	end
	
end