--[[

ItemWasteCommand Script

Notes: 

The param "invActionInfo" has the vars: actorId, unknown, slot, and inventoryType. 
The param "itemDBIds" has the vars: item1 and item2. 

--]]

function onEventStarted(player, actor, triggerName, itemReference, targetPackage, sourcePackage, arg1, arg2, unknown, arg3, arg4, arg5, type9ItemIds)
	player:GetItemPackage(itemReference.itemPackage):RemoveItemAtSlot(itemReference.slot);
	player:EndEvent();	
end
