--[[

ObjectEventDoor Script

Functions:

eventDoorMoveAsk() - Shows confirm to move into event

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)	
	choice = callClientFunction(player, "eventDoorMoveAsk");
	
	if (choice == 1) then
		
	end
	
	player:EndEvent();	
end