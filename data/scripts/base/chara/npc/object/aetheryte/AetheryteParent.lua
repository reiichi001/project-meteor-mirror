--[[

AetheryteParent Script

Functions:

eventAetheryteParentSelect(0x0, false, 0x60, 0x138807,0,0,0,0)
eventAetheryteParentDesion(
showAetheryteTips(
eventGLSelect(0)
eventSelectGLDetail(0x2a48, a, f4241, 136, 98b1d9, 1, 1, true, false)
eventGLDifficulty(0x2a48)
eventGLStart(0x2a48, 2, c8, 0, 0, 0, 0)
eventGLBoost()
eventGLPlay
eventGLReward()


Menu Ids:

--]]

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	player:RunEventFunction("eventAetheryteParentSelect", 0x0, false, 0x61, 0x0,0,0,0,0);
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)
	--player:RunEventFunction("askOfferQuest", player, 1000);
	player:EndEvent();
end