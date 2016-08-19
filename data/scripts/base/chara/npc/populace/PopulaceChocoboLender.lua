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

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	local curLevel = 20;
	local hasIssuance = true;
	local hasChocobo = player.hasChocobo;

	if (player.isGM and hasChocobo == false) then
		hasIssuance = true;
	end

	local rentPrice = 800;
	local playerFunds = 0; --TODO: pull character's money
	local hasFunds = (playerFunds >= rentPrice);

	callClientFunction(player, "eventTalkWelcome", player);
	menuChoice = callClientFunction(player, "eventAskMainMenu", player, curLevel, hasFunds, hasIssuance, hasChocobo, hasChocobo, 4);

	if (menuChoice == 1) then -- Issuance option
		callClientFunction(player, "eventTalkMyChocobo", player);
		nameResponse = callClientFunction(player, "eventSetChocoboName", false);

		if (nameResponse == "") then -- Cancel Chocobo naming
			callClientFunction(player, "eventCancelChocoboName", player);
		end

		appearance = 1; -- TODO: pull correct appearance based on GC
		--player:issueChocobo(appearance, nameResponse);
		if (nameResponse ~= "") then -- Successfully named Chocobo
			callClientFunction(player, "eventAfterChocoboName", player);
		end
	elseif(menuChoice == 2 and hasChocobo) then -- Summon Bird
		player:ChangeMusic(83);
		player:SendChocoboAppearance();
		player:SendGameMessage(player, worldMaster, 26001, 0x20);
		player:SetMountState(1);
	elseif(menuChoice == 3) then -- Change Barding
		callClientFunction(player, "eventTalkStepBreak", player);
	elseif(menuChoice == 5) then -- Rent Bird
		if (hasFunds == false) then -- Not enough money
			-- Do not enough money action??
		else
			--Issue rental chocobo
		end
	else
		callClientFunction(player, "eventTalkStepBreak", player);
	end

	player:EndEvent();
end