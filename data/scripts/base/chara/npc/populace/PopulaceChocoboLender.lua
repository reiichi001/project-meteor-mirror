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
	
	local curLevel = 20; -- TODO: pull from character
	local hasIssuance = true; -- TODO: pull from character
	local hasChocobo = player.hasChocobo;

	if (player.isGM and hasChocobo == false) then -- Let GMs auto have the issuance for debugging 
		hasIssuance = true;
	end

	if (hasChocobo) then
		hasIssuance = false;
	end

	local rentPrice = 800;
	local playerFunds = 0; --TODO: pull character's money
	local hasFunds = (playerFunds >= rentPrice);

	callClientFunction(player, "eventTalkWelcome", player);
	local menuChoice = callClientFunction(player, "eventAskMainMenu", player, curLevel, hasFunds, hasIssuance, hasChocobo, hasChocobo, 4);

	if (menuChoice == 1) then -- Issuance option
		callClientFunction(player, "eventTalkMyChocobo", player);
		local nameResponse = callClientFunction(player, "eventSetChocoboName", false);

		if (nameResponse == "") then -- Cancel Chocobo naming
			local cancelState = callClientFunction(player, "eventCancelChocoboName", player);
			--Do anything with cancel state?
		end

		local appearance = 1; -- TODO: pull correct appearance based on GC
		player:IssueChocobo(appearance, nameResponse);
		if (nameResponse ~= "") then -- Successfully named Chocobo
			callClientFunction(player, "eventAfterChocoboName", player);
		end
		
		mountChocobo(player);
		teleportOutOfCity(player);
	elseif(menuChoice == 2) then -- Summon Bird
		mountChocobo(player);
		teleportOutOfCity(player);
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

function mountChocobo(player)
	--TODO fix this
	--[[
	player:ChangeMusic(83);
	player:SendChocoboAppearance();
	player:SendGameMessage(player, worldMaster, 26001, 0x20);
	player:SetMountState(1);
	]]
end

function teleportOutOfCity(player) 
	--TODO: Teleport out of city
	local zoneId = player:GetPos()[4];
	local worldManager = GetWorldManager();
	if(zoneId == 155) then --Gridania
		worldManager:DoZoneChange(player, 150, nil, 0x02, 319, 4, -996, 0.00);
	elseif(zoneId == 133) then -- Limsa
		worldManager:DoZoneChange(player, 133, nil, 0x02, -73, 30, 169, 2);
	elseif(zoneId == 175) then -- Ul'dah

	end
end