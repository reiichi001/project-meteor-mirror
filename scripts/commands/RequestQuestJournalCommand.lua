--[[


--]]

function onEventStarted(player, actor, questId)
	player:sendRequestedInfo("requestedData", "qtdata", 0x1D4F2);
end
