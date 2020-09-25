require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)

	choice = callClientFunction(player, "askLogout", player);
	
	if (choice == 2) then
		player:SetSleeping();
		player:QuitGame();
	elseif (choice == 3) then
		player:SetSleeping();
		player:Logout();
	elseif (choice == 4) then
		player:SendMessage(33, "", "Heck the bed");
	end
	
	player:EndEvent();
	
end