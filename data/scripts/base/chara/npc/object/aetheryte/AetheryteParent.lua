--[[

AetheryteParent Script

Functions:

eventAetheryteParentSelect(0x0, false, 0x60, 0x138807,0,0,0,0)
eventAetheryteParentDesion(sheetId)
processGuildleveBoost(?, ?)
processGuildlevePlaying(??)
processGuildleveJoin() - Join party leader's levequest?

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	callClientFunction(player, "eventAetheryteParentSelect", 0x0, false, 0x61, 0x0,0,0,0,0);
	player:EndEvent();
end