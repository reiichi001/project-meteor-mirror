--[[

BazaarDealCommand Script

Handles various bazaar transfer options

All bazaar args have a Reward (The item the person who fufills the request gets) and a Seek (The item the player wants, either gil or an item).

--]]

function onEventStarted(player, actor, triggerName, rewardItem, seekItem, bazaarMode, arg1, bazaarActor, rewardAmount, seekAmount, arg2, arg3, type9ItemIds)

	local originalReward = nil;
	local originalSeek = nil;	

	--Handle special case for offers
	if (seekItem == nil) then
		originalSeek = player:GetItemPackage(0):GetItemAtSlot(rewardItem.seekSlot);
		originalReward = player:GetItemPackage(0):GetItemAtSlot(rewardItem.offerSlot);
	end	
	
	--Handle Reward
	if (type(rewardItem) == "number") then
		rewardItem = GetWorldManager():CreateItem(rewardItem, rewardAmount);
		player:RemoveItem(1000001, rewardAmount);
	elseif (seekItem == nil) then
		rewardItem = originalReward;
		if (bazaarMode ~= 11) then
			rewardItem = GetWorldManager():CreateItem(rewardItem.itemId, rewardAmount, rewardItem.quality, rewardItem.modifiers);
		end
	else
		rewardItem = player:GetItem(rewardItem);
		originalReward = rewardItem;
		if (bazaarMode ~= 11) then
			rewardItem = GetWorldManager():CreateItem(rewardItem.itemId, rewardAmount, rewardItem.quality, rewardItem.modifiers);
		end
	end
	
	--Handle Seek
	if (type(seekItem) == "number") then
		seekItem = GetWorldManager():CreateItem(seekItem, seekAmount);
	elseif (seekItem == nil) then
		seekItem = originalSeek;
		if (bazaarMode ~= 11) then
			seekItem = GetWorldManager():CreateItem(seekItem.itemId, seekAmount, seekItem.quality, seekItem.modifiers);
		end
	else
		seekItem = player:GetItem(seekItem);
		originalSeek = seekItem;
		if (bazaarMode ~= 11) then
			seekItem = GetWorldManager():CreateItem(seekItem.itemId, seekAmount, seekItem.quality, seekItem.modifiers);
		end
	end
	
	--If not selling, remove the seek item
	if (bazaarMode ~= 11 and bazaarMode ~= 12 and bazaarMode ~= 13) then
		if (originalSeek ~= nil) then
			player:RemoveItem(originalSeek, seekAmount);
		end
	end
	
	--Remove the reward item
	if (originalReward ~= nil) then
		player:RemoveItem(originalReward, rewardAmount);
	end
	
	GetWorldManager():AddToBazaar(player, rewardItem, seekItem, rewardAmount, seekAmount, bazaarMode);
	
	player:EndEvent();
	
end