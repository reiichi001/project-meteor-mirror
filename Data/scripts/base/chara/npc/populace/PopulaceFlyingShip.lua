--[[

PopulaceFlyingShip Script

Functions:

eventIn(player, hasTicket, nil?, travelPrice) - If hasTicket == nil, say no money text.
eventOut(isAborting) - Set isAborting to 30010 if player didn't "use" the airship. Shows no refund warning.
eventNG(player) - Message said when player is talking to the wrong npc.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	callClientFunction(player, "eventIn", player, false, nil, 5);
	
	player:EndEvent();
end