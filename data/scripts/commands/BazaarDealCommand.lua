--[[

BazaarDealCommand Script

Handles various bazaar transfer options

All bazaar args have a Reward (The item the person who fufills the request gets) and a Seek (The item the player wants, either gil or an item).

--]]

function onEventStarted(player, actor, triggerName, rewardItem, seekItem, bazaarMode, arg1, bazaarActor, rewardAmount, seekAmount, arg2, arg3, type9ItemIds)

	--Get reward reference or itemId
	
	--Get seek reference or itemid
	
	--Tell worldmaster to add bazaar entry with reward, seek, rewardAmount, seekAmount, and bazaarMode
	
	--Remove reward items from inventory
	
	player:EndEvent();
	
end