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

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
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
			teleportOutOfCity(player, npc);
		end
				
	elseif(menuChoice == 2) then -- Summon Bird
		teleportOutOfCity(player, npc);
		mountChocobo(player);
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
	local worldMaster = GetWorldMaster();
	player:ChangeMusic(83);
	player:SendChocoboAppearance();
	player:SendGameMessage(player, worldMaster, 26001, 0x20);
	player:SetMountState(1);
end

function issueRentalChocobo(player)
	--TODO: Write issue rental chocobo code
end

function teleportOutOfCity(player, npc) 
	local zoneId = player:GetPos()[4];
	local worldManager = GetWorldManager();
	local exitPoints = {
		[1500061] = {150, 319, 4, 996, 0.00}, -- Gridania
		[1500006] = {133, -83, 30, 169, 2.00}, -- Limsa
		[1000840] = {170, -32, 183, -74, 2} -- Ul'dah
	};
	--print "Getting exit point for npc [" ..npc:GetActorClassId().."]";
	local exitPoint = exitPoints[npc:GetActorClassId()];
	if (exitPoint == nil) then
		return
	end
	worldManager:DoZoneChange(player, exitPoint[0], nil, 0x02, exitPoint[1], exitPoint[2], exitPoint[3], exitPoint[4]);
end
