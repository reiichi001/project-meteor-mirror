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

Notes:

* Rent price and time seems to be hardcoded into the client. Price is always 800gil and time is 10m.
* The func eventSetChocoboName *requires* the actor with id `1080101` to be present in the client instance or it will crash (thanks Jorge for finding that).
* Special spawn codes must be sent for getting your chocobo or renting for it to work properly.

--]]

require ("global")

local rentalPrice = 800;
local rentalTime = 10;

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
	[1500006] = {133, -6.032, 46.356, 132.572, 3.034},
	[1500061] = {150, 333.271, 5.889, -943.275, 0.794},
	[1000840] = {170, -26.088, 181.846, -79.438, 2.579}
};

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)	
	local curLevel = 20; -- TODO: pull from character
	local hasIssuance = player:GetItemPackage(INVENTORY_KEYITEMS):HasItem(gcIssuances[npc:GetActorClassId()]);
	local hasChocobo = player.hasChocobo;
	
	if (hasChocobo == false) then -- Let GMs auto have the issuance for debugging 
		hasIssuance = true;
	end	

	local hasFunds = (player:GetCurrentGil() >= rentalPrice);

	callClientFunction(player, "eventTalkWelcome", player);
	
	local menuChoice = callClientFunction(player, "eventAskMainMenu", player, curLevel, hasFunds, hasIssuance, hasChocobo, hasChocobo, 0);

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

			--Add Chocobo License and remove issuance
			if (player:GetItemPackage(INVENTORY_KEYITEMS):HasItem(2001007) == false) then
				player:GetItemPackage(INVENTORY_KEYITEMS):AddItem(2001007);
			end
			player:GetItemPackage(INVENTORY_KEYITEMS):RemoveItem(gcIssuances[npc:GetActorClassId()], 1);
			
			--Warp with the special chocobo warp mode.
			mountChocobo(player);			
			GetWorldManager():DoZoneChange(player, cityExits[npc:GetActorClassId()][1], nil, 0, SPAWN_CHOCOBO_GET, cityExits[npc:GetActorClassId()][2], cityExits[npc:GetActorClassId()][3], cityExits[npc:GetActorClassId()][4], cityExits[npc:GetActorClassId()][5]);		
		end
				
	elseif(menuChoice == 2) then -- Summon Bird
		mountChocobo(player);
		GetWorldManager():DoZoneChange(player, cityExits[npc:GetActorClassId()][1], nil, 0, SPAWN_NO_ANIM, cityExits[npc:GetActorClassId()][2], cityExits[npc:GetActorClassId()][3], cityExits[npc:GetActorClassId()][4], cityExits[npc:GetActorClassId()][5]);		
	elseif(menuChoice == 3) then -- Change Barding
		callClientFunction(player, "eventTalkStepBreak", player);
	elseif(menuChoice == 5) then -- Rent Bird
		mountChocobo(player, true, 1);
		GetWorldManager():DoZoneChange(player, cityExits[npc:GetActorClassId()][1], nil, 0, SPAWN_CHOCOBO_RENTAL, cityExits[npc:GetActorClassId()][2], cityExits[npc:GetActorClassId()][3], cityExits[npc:GetActorClassId()][4], cityExits[npc:GetActorClassId()][5]);
	else
		callClientFunction(player, "eventTalkStepBreak", player);
	end

	player:EndEvent();
end

function mountChocobo(player, isRental, rentalMinutes)
	if (isRental) then		
		player:ChangeMusic(64);		
		player:StartChocoboRental(rentalMinutes);
	else
		player:ChangeMusic(83);
	end
	
	player:SendMountAppearance();
	player:SetMountState(1);
	player:ChangeSpeed(0.0, 5.0, 10.0, 10.0);
	player:ChangeState(15);	
end