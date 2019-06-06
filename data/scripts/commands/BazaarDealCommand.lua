--[[

BazaarDealCommand Script

Handles various bazaar transfer options

All bazaar args have a Reward (The item the person who fufills the request gets) and a Seek (The item the player wants, either gil or an item).

Args:

rewardItem: Item reference to what will be given to the buyer. If it's gil the itemID will be given instead. If offering an item to seek; reward/seek are combined and put here.
seekItem: Item reference to what the buyer will give us. If it's gil the itemID will be given instead,
bazaarMode: The tag value to set in the bazaar item's data.
arg1: Always nil
bazaarActor: The actor who owns this bazaar
rewardAmount: The amount of rewardItem the buyer will get.
seekAmount: The amount of seekItem we want.

--]]

require ("global")

function onEventStarted(player, actor, triggerName, rewardItem, seekItem, bazaarMode, arg1, bazaarActor, rewardAmount, seekAmount, arg2, arg3, type9ItemIds)

	local rewarding = nil;
	local seeking = nil;

	--Handle special case for offering an item.
	if (seekItem == nil) then
		rewarding = player:GetItemPackage(rewardItem.offerPackageId):GetItemAtSlot(rewardItem.offerSlot);
		seeking = player:GetItemPackage(rewardItem.seekPackageId):GetItemAtSlot(rewardItem.seekSlot);
	end	
	
	--Handle Reward
	if (rewarding == nil) then
		if (type(rewardItem) == "number") then
			rewarding = player:GetItemPackage(INVENTORY_CURRENCY):GetItemByCatelogId(rewardItem);
		else
			rewarding = player:GetItem(rewardItem);			
		end
	end
	
	--Handle Seek
	if (seeking == nil) then
		if (type(seekItem) == "number") then
			seeking = player:GetItemPackage(INVENTORY_CURRENCY):GetItemByCatelogId(seekItem);
		else
			seeking = player:GetItem(seekItem);			
		end
	end
		
	result = GetWorldManager():AddToBazaar(player, rewarding, seeking, rewardAmount, seekAmount, bazaarMode);

	

	player:EndEvent();
	
end