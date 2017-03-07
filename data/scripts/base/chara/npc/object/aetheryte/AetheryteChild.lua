--[[

AetheryteChild Script

Functions:

eventAetheryteChildSelect(showTeleport, parentAetheryteID, animaAmount, animaCost(always 1)): Opens menu
eventAetheryteChildDesion(aetheryteId): "Your homepoint is now X"
processGuildleveBoost(favourCost, currentFavour): Ask: "Invoke the aspect of your Guardian deity to gain a temporary boon for the duration of a levequest."
processGuildlevePlaying(??)
processGuildleveJoin() - Ask: "Do you wish to join your party leader's levequest?"

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	menuChoice = callClientFunction(player, "eventAetheryteChildSelect", true, 1280062, 4, 1);
	
	--Teleport
	if (menuChoice == 2) then
		
	--Init Levequest
	elseif (menuChoice == -1) then
		callClientFunction(player, "eventGLSelect", 0);
	--Set Homepoint
	elseif (menuChoice == -2) then
	
	--View Faction Standing
	elseif (menuChoice == -3) then
	
	end
	
	player:EndEvent();
end

