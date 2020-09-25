--[[

LogoutCommand Script

Functions:

eventConfirm()
eventCountDown()
eventLogoutFade()

--]]

require ("global")

function onEventStarted(player, command, triggerName)

	choice = callClientFunction(player, "delegateCommand", command, "eventConfirm");	

	if (choice == 1) then
		player:QuitGame();
	elseif (choice == 2) then
		player:Logout();
	end
	
	player:EndEvent();
	
end
