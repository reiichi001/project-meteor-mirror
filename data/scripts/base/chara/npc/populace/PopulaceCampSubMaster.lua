--[[

PopulaceCampSubMaster Script

Functions:

talkWelcome(player, level, ?) - Main npc function.
confirmUseFacility(player, gilAmount) - Confirm dialog if player uses facility.
finishTalkTurn() - Call to stop the npc staring at player.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	choice = callClientFunction(player, "talkWelcome", player, 1, false);
	
	if (choice == 1) then
		confirmed = callClientFunction(player, "confirmUseFacility", player, 1);
	end
	
	callClientFunction(player, "finishTalkTurn");
	
	player:EndEvent();
	
end