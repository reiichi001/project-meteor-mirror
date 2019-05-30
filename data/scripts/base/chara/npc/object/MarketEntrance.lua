--[[

MarketEntrance Script

Functions:

eventPushChoiceAreaOrQuest(gcLeaderPlaceName[Fronds, etc], showMarketWards/Houses (must be 0xc1a), gcHQPlaceName, anotherPlaceName, showItemSearchCounter, stopSearchingItemId) - 
eventPushStepPrvMarket(?, ?, ?) -

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;
end

function onEventStarted(player, npc, triggerName)	
	callClientFunction(player, "eventPushChoiceAreaOrQuest", 0xc13, 0xc1a, 0xdba, 0, true, 1);
	player:EndEvent();	
end