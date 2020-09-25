--[[

ObjectItemStorage Script

Functions:

storageMenu() - Shows store/retrieve/help menu.
selectCategory() - Shows the category menu
selectStoreItem(nil, categoryId) - Shows store item menu
selectReceiveItem(nil, categoryId) - Shows retrieve item menu
--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)	
	
	::TOP_MENU::
	storageChoice = callClientFunction(player, "storageMenu");
	
	if (storageChoice == 1) then
		categoryChoice = callClientFunction(player, "selectCategory");
		
		if (categoryChoice == 5) then
			goto TOP_MENU;
		end
		
		itemId = callClientFunction(player, "selectStoreItem", nil, categoryChoice);
			
		if (itemId ~= nil) then
			player:GetItemPackage(INVENTORY_NORMAL):RemoveItem(itemId, 1);
		end
		
	elseif (storageChoice == 2) then
		categoryChoice = callClientFunction(player, "selectCategory");
	
		if (categoryChoice == 5) then
			goto TOP_MENU;
		end
	
		itemId = callClientFunction(player, "selectReceiveItem", nil, categoryChoice);
		
		if (itemId ~= nil) then
			player:GetItemPackage(INVENTORY_NORMAL):AddItem(itemId, 1);
		end
	
	end
	
	player:EndEvent();
	
end 