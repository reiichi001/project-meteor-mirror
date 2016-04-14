--[[

LogoutCommand Script

Functions:

eventConfirm()
eventCountDown()
eventLogoutFade()

Menu Ids:

Menu: 		0	
Countdown: 	1

--]]

function onEventStarted(player, command)
	--player:setCurrentMenuId(0);
	--player:runEventFunction("delegateCommand", command, "eventConfirm");
	player:logout();
end

function onEventUpdate(player, command, triggerName, step, arg1, arg2)

	currentMenuId = player:getCurrentMenuId();

	--Menu Dialog	
	if (currentMenuId == 0) then
		if (arg1 == 1) then --Exit	
			player:quitGame();
			player:endCommand();	
		elseif (arg1 == 2) then --Character Screen
			player:logout();
			player:endCommand();
			--player:setCurrentMenuId(1);
			--player:runEventFunction("delegateCommand", command, "eventCountDown");
		elseif (arg1 == 3) then --Cancel
			player:endCommand();
		end
	--Countdown Dialog
	elseif (currentMenuId == 1) then
	
		if (arg2 == 1) then --Logout Complete
			player:logout();
			player:endCommand();			
		elseif (arg2 == 2) then --Cancel Pressed
			player:endCommand();
		end
		
	end
	
end