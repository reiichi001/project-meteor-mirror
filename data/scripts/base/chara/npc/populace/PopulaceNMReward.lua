--[[

PopulaceNMReward Script

Functions:

eventTalkStep0(player, ?, ?) - Opens the main menu
eventTalkStep0_1(player) - "Ain't running a charity here", message said when you have insufficent funds
eventTalkStep0_2(player, hasItems) - Relic Quest dialog.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "eventTalkStep0", player, 0);
	
	player:EndEvent();
end