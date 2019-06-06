--[[

Common Retainer Stuff

Retainer Say Codes:

1: Hired
2: When called
3: Error when cannot call retainer
4: Dismissed
5: ??
6: Sold X items report.
7: Nothing got sold.
8: Retainer payed???
9: Retainer dismissed due to not paid.
10: Retainer dismissed by player.


--]]

function doItemTrade(player, retainer)
	callClientFunction(player, "eventTalkRetainerItemTrade", 1);
	
	while (true) do
		resultCode, item, un1, quantity, itemId, quality = callClientFunction(player, "eventTalkRetainerItemTrade", 2);		
		
		player:SendMessage(0x20, "", "" .. tostring(resultCode));
		player:SendMessage(0x20, "", "" .. tostring(un1));
		player:SendMessage(0x20, "", "" .. tostring(quantity));
		player:SendMessage(0x20, "", "" .. tostring(itemId));
		player:SendMessage(0x20, "", "" .. tostring(quality));
	
		--Retreieve
		if (resultCode == 31) then		
			retainer:GetItemPackage(item.itemPackage):RemoveItemAtSlot(item.slot, quantity);
			retainer:GetItemPackage(item.itemPackage):SendUpdatePackets(player, true);
			player:GetItemPackage(item.itemPackage):AddItem(itemId, quantity, quality);
		--Entrust
		elseif (resultCode == 32) then					
			player:GetItemPackage(item.itemPackage):RemoveItemAtSlot(item.slot, quantity);
			retainer:GetItemPackage(item.itemPackage):AddItem(itemId, quantity, quality);
			retainer:GetItemPackage(item.itemPackage):SendUpdatePackets(player, true);
		end
		
		callClientFunction(player, "eventReturnResult", resultCode, false);
		
		if (resultCode == 100) then
			break
		end
	end
	
	callClientFunction(player, "eventTalkRetainerItemTrade", 3);
end

function doBazaar(player, retainer)
	callClientFunction(player, "eventTalkRetainerItemList", 1);
	callClientFunction(player, "eventTalkRetainerItemList", 2);	
	callClientFunction(player, "eventTalkRetainerItemList", 3);
end