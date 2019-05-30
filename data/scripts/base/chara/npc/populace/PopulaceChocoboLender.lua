--[[

PopulaceChocoboLender Script

Functions:

eventTalkWelcome(player) - Start Text
eventAskMainMenu(player, curLevel, hasFundsForRent, isPresentChocoboIssuance, isSummonMyChocobo, isChangeBarding, currentChocoboWare) - Shows the main menu
eventTalkMyChocobo(player) - Starts the cutscene for getting a chocobo
eventSetChocoboName(true) - Opens the set name dialog
eventAfterChocoboName(player) - Called if player done naming chocobo, shows cutscene, returns state and waits to teleport outside city.
eventCancelChocoboName(player) - Called if player cancels naming chocobo, returns state. 
eventTalkStepBreak(player) - Finishes talkTurn and says a goodbye
--]]

require ("global")

local gcIssuances = {
	[1500006] = 2001004,
	[1500061] = 2001005,
	[1000840] = 2001006
};

local startAppearances = {
	[1500006] = CHOCOBO_LIMSA1,
	[1500061] = CHOCOBO_GRIDANIA1,
	[1000840] = CHOCOBO_ULDAH1
};

local cityExits = {
	[1500006] = 15,
	[1500061] = 14,
	[1000840] = 16
};

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	--callClientFunction(player, "eventTalkWelcome", player);
	--callClientFunction(player, "eventAskMainMenu", player, 20, true, true, true, true, 4);
	--callClientFunction(player, "eventTalkMyChocobo", player);
	--callClientFunction(player, "eventSetChocoboName", false);
	--callClientFunction(player, "eventAfterChocoboName", player);

	local curLevel = 20; -- TODO: pull from character
	local hasIssuance = player:GetInventory(INVENTORY_KEYITEMS):HasItem(gcIssuances[npc:GetActorClassId()]);
	local hasChocobo = player.hasChocobo;
	
	if (player.isGM and hasChocobo == false) then -- Let GMs auto have the issuance for debugging 
		hasIssuance = true;
	end	

	local rentPrice = 800;
	local hasFunds = (player:GetCurrentGil() >= rentPrice);

	callClientFunction(player, "eventTalkWelcome", player);
	
	local menuChoice = callClientFunction(player, "eventAskMainMenu", player, curLevel, hasFunds, hasIssuance, true, true, player.chocoboAppearance);

	if (menuChoice == 1) then -- Issuance option
		callClientFunction(player, "eventTalkMyChocobo", player);
		local nameResponse = callClientFunction(player, "eventSetChocoboName", true);

		if (nameResponse == "") then -- Cancel Chocobo naming
			callClientFunction(player, "eventCancelChocoboName", player);
			callClientFunction(player, "eventTalkStepBreak", player);
			player:EndEvent();
			return;
		else		
			local appearance = startAppearances[npc:GetActorClassId()];
			player:IssueChocobo(appearance, nameResponse);
			callClientFunction(player, "eventAfterChocoboName", player);
			mountChocobo(player);
			GetWorldManager():DoZoneChange(player, cityExits[npc:GetActorClassId()]);			
			player:SendGameMessage(player, GetWorldMaster(), 25248, 0x20, 2001007);
			player:SendDataPacket("attention", GetWorldMaster(), "", 25248, 2001007);
			
			if (player:GetInventory(INVENTORY_KEYITEMS):HasItem(2001007) == false) then
				player:GetInventory(INVENTORY_KEYITEMS):AddItem(2001007);
			end
			
			player:GetInventory(INVENTORY_KEYITEMS):RemoveItem(gcIssuances[npc:GetActorClassId()], 1);
			
			player:EndEvent();
			return;
		end
				
	elseif(menuChoice == 2) then -- Summon Bird
		mountChocobo(player);
		GetWorldManager():DoZoneChange(player, cityExits[npc:GetActorClassId()]);
	elseif(menuChoice == 3) then -- Change Barding
		callClientFunction(player, "eventTalkStepBreak", player);
	elseif(menuChoice == 5) then -- Rent Bird
		issueRentalChocobo(player);
	else
		callClientFunction(player, "eventTalkStepBreak", player);
	end

	player:EndEvent();
end

function mountChocobo(player)
	player:SendChocoboAppearance();
	player:SetMountState(1);
	player:ChangeSpeed(0.0, 5.0, 10.0);
	player:ChangeState(15);
end

function issueRentalChocobo(player)
	--TODO: Write issue rental chocobo code
end
