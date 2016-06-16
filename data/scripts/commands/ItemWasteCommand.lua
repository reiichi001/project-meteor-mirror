--[[

ItemWasteCommand Script

Notes: 

The param "invActionInfo" has the vars: actorId, unknown, slot, and inventoryType. 
The param "itemDBIds" has the vars: item1 and item2. 

--]]

function onEventStarted(player, actor, triggerName, invActionInfo, param1, param2, param3, param4, param5, param6, param7, param8, itemDBIds)
	player:GetInventory(0x00):RemoveItem(invActionInfo.slot);
	player:EndCommand();	
end
