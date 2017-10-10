--[[

MarketEntrance Script

Functions:

eventPushChoiceAreaOrQuest(0xc13, 0xc1a, 0xdba,0, false, 0) - 
eventPushStepPrvMarket(?, ?, ?) -

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;
end

function onEventStarted(player, npc, triggerName)	
	callClientFunction(player, "eventPushChoiceAreaOrQuest", 0xc13, 0xc1a, 0xdba,0, false, 0);
		
	player:EndEvent();	
end