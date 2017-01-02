--[[

ConfirmGroupCommand Script

Handles what happens when you resolve an invite to a group

--]]

function onEventStarted(player, actor, triggerName, groupType, result)

	--Accept/Refuse happened, else just close the window
	if (result == 1 or result == 2) then
		GetWorldManager():GroupInviteResult(player, groupType, result);		
	end
	
	player:EndEvent();
	
end