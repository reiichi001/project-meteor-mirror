--[[

PopulaceItemRepairer Script

Functions:

talkWelcome(player, sayWelcomeText, currentLevel?, changes 1500243 from "welcome" to "well met") - Opens the main menu
selectItem(nil, pageNumber, ?, condition1, condition2, condition3, condition4, condition5) - Select item slot.
confirmRepairItem(player, price, itemId, hq grade) - Shows the confirm box for item repair.
confirmUseFacility(player, price) - Shows confirm box for using facility. Default price is 11k?
finishTalkTurn() - Call at end to stop npc from staring at the player (eeeek)

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)	
	
	result = callClientFunction(player, "talkWelcome", player, true, 20, false);
	
	if (result == 1) then	
		local currentPage = 1;
		local slotToRepair = nil;
		
		while (true) do
			slot, page, listIndx = callClientFunction(player, "selectItem", nil, currentPage, 4, 2, 55, 55, 55, 55);
			
			if (slot == nil and page ~= nil) then
				currentPage = page;				
			else
				slotToRepair = slot;
				break;
			end			
		end
		
		if (slotToRepair ~= nil) then
			callClientFunction(player, "confirmRepairItem", player, 100, 8032827, 0);
		end
		
	elseif (result == 2) then	
		callClientFunction(player, "confirmUseFacility", player);
	end
	
	callClientFunction(player, "finishTalkTurn");
	
	player:EndEvent();
end