--[[


--]]

function onEventStarted(player, actor, questId)
	player:sendRequestedInfo("requestedData", "activegl", 7, nil, nil, nil, nil, nil, nil, nil);
--	player:sendRequestedInfo("requestedData", "glHist", 10, 0x1D4F2, 1009, 12464, 11727, 12485, 12526);
end
