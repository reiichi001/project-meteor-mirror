--[[

PopulaceShopSalesman Script

Functions:

welcomeTalk(sheetId, player) - Start Message
selectMode(askMode) - Shows buy/sell modes. If askmode > 0 show guild tutorial. If askmode == -7/-8/-9 show nothing. Else show affinity/condition tutorials.
selectModeOfClassVendor() - Opens categories for class weapons and gear
selectModeOfMultiWeaponVendor(consumptionTutorialId) - Opens categories for weapons/tools (war/magic/land/hand). Arg consumptionTutorialId appends location of item repair person. -1: Ul'dah, -2: Gridania, -3: Limsa 
selectModeOfMultiArmorVendor(consumptionTutorialId) - Opens categories for armor in different slots. Arg consumptionTutorialId appends location of item repair person. -1: Ul'dah, -2: Gridania, -3: Limsa 

openShopBuy(player, shopPack, currancyItemId) - ShopPack: Items to appear in window. CurrancyItemId: What is being used to buy these items.
selectShopBuy(player) - Call after openShopBuy() to open widget
closeShopBuy(player) - Closes the buy window

openShopSell(player) - Call this to open sell window
selectShopSell(player) - Call after openShopSell()
closeShopSell(player) - Closes the sell window

selectFacility(?, sheetId, 3) - Opens the facility chooser.
confirmUseFacility(player, cost) - Facility cost confirm

informSellPrice(1, chosenItem, price) - Shows sell confirm window. ChosenItem must be correct.

startTutorial(nil, tutorialId) - Opens up a tutorial menu for each guild type based on tutorialId

finishTalkTurn() - Done at the end.

--]]

function callClientFunction(player, functionName, ...)
	player:RunEventFunction(functionName, ...);
	result = coroutine.yield();
	return result;
end

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)

	require("/unique/".. npc.zone.zoneName .."/PopulaceShopSalesman/" .. npc:GetUniqueId())
	
	callClientFunction(player, "welcomeTalk", shopInfo.welcomeText, player);	
	
	while (true) do
		choice = callClientFunction(player, "selectMode", 1);
				
		if (choice == nil) then
			break;	
		elseif (choice == 1) then
			callClientFunction(player, "openShopBuy", player, shopInfo.shopPack, shopInfo.shopCurrancy);
			
			while (true) do		
				buyResult = callClientFunction(player, "selectShopBuy", player);
				
				if (buyResult == 0) then
					callClientFunction(player, "closeShopBuy", player);					
					break;
				else
					player:SendMessage(0x20, "", "Player bought a thing at slot " .. tostring(buyResult)..".");
				end
				
			end
			
		elseif (choice == 2) then
			callClientFunction(player, "openShopSell", player);

			while (true) do		
				sellResult = callClientFunction(player, "selectShopSell", player);
				
				if (sellResult == nil) then
					callClientFunction(player, "closeShopSell", player);					
					break;
				else
					player:SendMessage(0x20, "", "Player sold a thing at slot " .. tostring(sellResult)..".");
				end
				
			end
		elseif (choice == 3) then
			callClientFunction(player, "selectFacility", 2, 35, 3);
			callClientFunction(player, "confirmUseFacility", player, 35);			
		elseif (choice == 4) then
			callClientFunction(player, "startTutorial", nil, 29);			
		end
	end
	
	callClientFunction(player, "finishTalkTurn", player);
	player:EndEvent();
	
end
