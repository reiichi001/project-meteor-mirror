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
	--player:SetCurrentMenuId(0);
	--player:RunEventFunction("delegateCommand", command, "eventConfirm");
	player:Logout();
end

function onEventUpdate(player, command, triggerName, step, arg1, arg2)

	currentMenuId = player:GetCurrentMenuId();

	--Menu Dialog	
	if (currentMenuId == 0) then
		if (arg1 == 1) then --Exit	
			player:QuitGame();
			player:EndCommand();	
		elseif (arg1 == 2) then --Character Screen
			player:Logout();
			player:EndCommand();
			--player:SetCurrentMenuId(1);
			--player:RunEventFunction("delegateCommand", command, "eventCountDown");
		elseif (arg1 == 3) then --Cancel
			player:EndCommand();
		end
	--Countdown Dialog
	elseif (currentMenuId == 1) then
	
		if (arg2 == 1) then --Logout Complete
			player:Logout();
			player:EndCommand();			
		elseif (arg2 == 2) then --Cancel Pressed
			player:EndCommand();
		end
		
	end
	
end