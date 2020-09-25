--[[

PopulaceBlackMarketeer Script

Functions:

eventTalkWelcome(player)                        - Start Text
eventSellItemAsk(player, itemName, tradePrice)  - Requires GC Affiliation. Trade menu for Commemorative Coin
eventAskMainMenu(player, index)                 - Shows menu prompt to purchase with gil or with GC seals
eventTalkBye(player)                            - Says bye text
eventTalkStepBreak()                            - Ends talk, NPC turns to face original position

eventSealShopMenuOpen()                         - Opens menu for purchasing with grand company seals
eventSealShopMenuAsk()                          - Returns two values, one that seems to always be true, and an index of purchased item
eventSealShopMenuClose()                        - Closes seal menu
eventGilShopMenuOpen()                          - Opens menu for purchasing with gil
eventGilShopMenuAsk()                           - Returns two values, one that seems to always be true, and an index of purchased item
eventGilShopMenuClose()                         - Closes gil menu

Class applies to only three NPCs
Actorclass Id - 1500293 : Momoroon, Limsa Lominsa
Actorclass Id - 1500294 : Gagaroon, Gridania
Actorclass Id - 1500295 : Lalaroon, Ul'dah

--]]

require ("global")
require ("shop")

shopInfo = { -- [ index ] = { itemId, gilPrice, sealPrice, city, itemCategory }
[1001] = {3020202, 100, 10000, 1, 1},
[1002] = {3020509, 400, 40000, 1, 1},
[1003] = {3020510, 400, 40000, 1, 1},
[1004] = {3020504, 1000, 100000, 1, 1},
[1005] = {3020505, 1000, 100000, 1, 1},
[1101] = {9040018, 1500, 150000, 1, 2},
[1102] = {9010025, 2000, 200000, 1, 2},
[1301] = {2001014, 4000, 400000, 1, 4},
[2001] = {3020203, 100, 10000, 2, 1},
[2002] = {3020509, 400, 40000, 2, 1},
[2003] = {3020510, 400, 40000, 2, 1},
[2004] = {3020504, 1000, 100000, 2, 1},
[2005] = {3020505, 1000, 100000, 2, 1},
[2101] = {9040018, 1500, 150000, 2, 2},
[2102] = {9010025, 2000, 200000, 2, 2},
[2301] = {2001015, 4000, 400000, 2, 4},
[3001] = {3020204, 100, 10000, 3, 1},
[3002] = {3020509, 400, 40000, 3, 1},
[3003] = {3020510, 400, 40000, 3, 1},
[3004] = {3020504, 1000, 100000, 3, 1},
[3005] = {3020505, 1000, 100000, 3, 1},
[3101] = {9040018, 1500, 150000, 3, 2},
[3102] = {9010025, 2000, 200000, 3, 2},
[3301] = {2001016, 4000, 400000, 3, 4},
}

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)

    commemorativeCoin = 10011251;
    commemorativeCoinValue = 25000;
    gilCurrency = 1000001;
    playerGC = player.gcCurrent
    playerGCSeal = 1000200 + playerGC;

	callClientFunction(player, "eventTalkWelcome", player);
	
    if player:GetItemPackage(INVENTORY_NORMAL):HasItem(commemorativeCoin) and playerGC > 0 then
        -- Checks for player having a commemorative coin, show window trade option if so.
        coinChoice = callClientFunction(player, "eventSellItemAsk", player, commemorativeCoin, commemorativeCoinValue);
        if coinChoice == 1 then
            currencyType = callClientFunction(player, "eventAskMainMenu", player);
        elseif coinChoice == 2 then
            -- You trade <itemQuantity1> <itemName1> <itemQuality1> for <itemQuantity2> <itemName2> <itemQuality2>.
            player:SendGameMessage(player, GetWorldMaster(), 25071, MESSAGE_TYPE_SYSTEM, commemorativeCoin, 1, playerGCSeal, 1, 1, commemorativeCoinValue);
            player:GetItemPackage(INVENTORY_NORMAL):RemoveItem(commemorativeCoin, 1);
            player:GetItemPackage(INVENTORY_CURRENCY):addItem(playerGCSeal, 25000, 1)
            -- TODO: Add handling for checking GC seals limit and not going over it
        end
    else
        -- If no grand company alignment, go straight to the shop that uses gil, otherwise show gc seal option.
        if playerGC == 0 then
            processGilShop(player);   
        else
            currencyType = callClientFunction(player, "eventAskMainMenu", player);
            if currencyType == 1 then
                processGilShop(player);    
            elseif currencyType == 2 then
                processSealShop(player); 
            end
        end
    end
    
    callClientFunction(player, "eventTalkBye", player);
	callClientFunction(player, "eventTalkStepBreak", player);
	player:EndEvent();
end


function processGilShop(player)

    callClientFunction(player, "eventGilShopMenuOpen", player);

    while (true) do     
        unk1, buyResult = callClientFunction(player, "eventGilShopMenuAsk", player);
        printf(unk1);
        if (buyResult == 0 or buyResult == -1) then
            callClientFunction(player, "eventGilShopMenuClose", player);                 
            break;
        else
            if shopInfo[buyResult] == nil then
                -- Prevent server crash from someone trying to buy a non-existent item via packet injection.
                break;
            else
                -- TODO: Add handling to check you're on the right NPC to prevent packet injecting a purchase from anything in the list
                if shopInfo[buyResult][5] == 4 then
                    location = INVENTORY_KEYITEMS;
                else    
                    location = INVENTORY_NORMAL;  
                end
                
                purchaseItem(player, location, shopInfo[buyResult][1], 1, 1, shopInfo[buyResult][3], gilCurrency);  
            end
        end   
    end
end


function processSealShop(player)

    callClientFunction(player, "eventSealShopMenuOpen", player);
    
    while (true) do     
        unk1, buyResult = callClientFunction(player, "eventSealShopMenuAsk", player);  
        
        if (buyResult == 0 or buyResult == -1) then
            callClientFunction(player, "eventSealShopMenuClose", player);
            break;
        else
            if shopInfo[buyResult] == nil then
                -- Prevent server crash from someone trying to buy a non-existent item via packet injection.
                break;
            else
                -- TODO: Add handling to check you're on the right NPC to prevent packet injecting a purchase from anything in the list
                if shopInfo[buyResult][5] == 4 then
                    location = INVENTORY_KEYITEMS;
                else    
                    location = INVENTORY_NORMAL;  
                end
            
                purchaseItem(player, location, shopInfo[buyResult][1], 1, 1, shopInfo[buyResult][2], playerGCSeal);  
            end
        end   
    end
end

