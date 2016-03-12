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
	player:setCurrentMenuId(0);
	player:runEventFunction("delegateCommand", command, "eventConfirm");
end

function onEventUpdate(player, command, step, arg1, arg2)

	currentMenuId = player:getCurrentMenuId();

	--Menu Dialog	
	if (currentMenuId == 0) then
		if (arg1 == 1) then --Exit	
			player:quitGame();
			player:endEvent();	
		elseif (arg1 == 2) then --Character Screen
			player:logout();
			player:endEvent();
			--player:setCurrentMenuId(1);
			--player:runEventFunction("delegateCommand", command, "eventCountDown");
		elseif (arg1 == 3) then --Cancel
			player:endEvent();
		end
	--Countdown Dialog
	elseif (currentMenuId == 1) then
	
		if (arg2 == 1) then --Logout Complete
			player:logout();
			player:endEvent();			
		elseif (arg2 == 2) then --Cancel Pressed
			player:endEvent();
		end
		
	end
	
end