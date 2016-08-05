--[[

PopulaceCompanyWarp Script

Functions:

eventTalkWelcome(player) - Start Text
eventAskMainMenu(player, index) - Shows teleport menu
eventAfterWarpOtherZone(player) - Fades out for warp
eventTalkStepBreak() - Ends talk
--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "eventAskMainMenu", player, 1);
	
	player:EndEvent();
end