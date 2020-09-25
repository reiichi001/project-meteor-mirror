--[[

BazaarUndealCommand Script

Handles canceling bazaar items

25107 - Your bazaar is either full or already contains that unique item.
25111 - Unable to complete transaction.
25112 - You are unable to remove the item from your bazaar. You cannot hold any more items.
25113 - Offered and sought items cannot be identical.
25114 - Items in less than mint condition cannot be offered.
25115 - Items in less than mint condition cannot be placed in your bazaar.

--]]

function onEventStarted(player, actor, triggerName, rewardItem, seekItem, bazaarType, narg, bazaarActor, rewardAmount, seekAmount, narg, narg, type9ItemIds)
	
	GetWorldManager():RemoveFromBazaar(player, player:GetItem(rewardItem));
		
	player:EndEvent();
	
end