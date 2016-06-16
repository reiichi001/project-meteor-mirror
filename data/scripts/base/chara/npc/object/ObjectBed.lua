
function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	player:RunEventFunction("askLogout", player);
end

function onEventUpdate(player, npc, eventStep, menuOptionSelected)	
	
	if (menuOptionSelected == 1) then 
		player:EndEvent();
		return;
	elseif (menuOptionSelected == 2) then
		player:QuitGame();
	elseif (menuOptionSelected == 3) then
		player:Logout();
	elseif (menuOptionSelected == 4) then
		player:SendMessage(33, "", "Heck the bed");
	end 	
	
	player:EndEvent();
		
end