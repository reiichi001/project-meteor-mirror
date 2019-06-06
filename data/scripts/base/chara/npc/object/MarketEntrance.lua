--[[

MarketEntrance Script

Functions:

eventPushChoiceAreaOrQuest(gcLeaderPlaceName[Fronds, etc], showMarketWards/Houses (must be 0xc1a), gcHQPlaceName, anotherPlaceName, showItemSearchCounter, stopSearchingItemId) - 
eventPushStepPrvMarket(?, ?, ?) -

--]]

require ("global")

local MARKETWARD_ENTRANCE = {-201.0, 0.0, -160.0, 1.5};

function init(npc)
	return false, false, 0, 0;
end

function onEventStarted(player, npc, triggerName)	
	callClientFunction(player, "eventPushChoiceAreaOrQuest", 0xc13, 0xc1a, 0xdba, 0, true, 1);
	player:EndEvent();	
end