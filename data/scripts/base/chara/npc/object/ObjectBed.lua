
function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	player:runEventFunction("askLogout", player);
end

function onEventUpdate(player, npc, eventStep, menuOptionSelected)	
	
	if (menuOptionSelected == 1) then 
		player:endEvent();
		return;
	elseif (menuOptionSelected == 2) then
		player:quitGame();
	elseif (menuOptionSelected == 3) then
		player:logout();
	elseif (menuOptionSelected == 4) then
		player:sendMessage(33, "", "Heck the bed");
	end 	
	
	player:endEvent();
		
end