require ("global")

--[[

NpcLinkshellChatCommand Script

Handler for when a player clicks a npc ls to talk to. If adding new linkshells to the handle, make sure to add
it to the handler table (with correct offset), and that your function is above the handler. If padding is needed
to hit some ID, add "nils".

--]]


local function handleAdventurersGuild(player)
	if (player:HasQuest(110006) == true) then
		local man0g1Quest = player:GetQuest("Man0g1");		
		player:SendGameMessage(man0g1Quest, 330, 39, 1300018, nil);
	end
end

local function handlePathOfTheTwelve(player)
	player:SendMessage(0x20, "", "Test");
end

local npcLsHandlers = {
	handleAdventurersGuild,
	nil,
	nil,
	nil,
	nil,
	handlePathOfTheTwelve	
}

function onEventStarted(player, command, triggerName, npcLsId)		
	
	if (npcLsHandlers[npcLsId] ~= nil) then
		npcLsHandlers[npcLsId](player);
		player:SetNpcLS(npcLsId-1, NPCLS_ACTIVE);
	else
		player:SendMessage(0x20, "", "That Npc Linkshell is not implemented yet.");
	end	
	
	player:endEvent();	
	
end
