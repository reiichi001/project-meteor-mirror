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

confirmSellingItem(itemId, quality, quantity, gil) - Simple Sell confirmation window

selectFacility(?, sheetId, 3) - Opens the facility chooser.
confirmUseFacility(player, cost) - Facility cost confirm

informSellPrice(1, chosenItem, price) - Shows sell confirm window. ChosenItem must be correct.

startTutorial(nil, tutorialId) - Opens up a tutorial menu for each guild type based on tutorialId

finishTalkTurn() - Done at the end.

--]]

require ("global")
require ("shop")

shopInfo = { -- [actorclass id] = { welcomeText, shopMode, shopPack{s} }
[1000159] = {34, 0, 1016},
[1000163] = {49, 0, 1017},
[1000165] = {74, 0, 1019},
[1001458] = {44, 0, 1018},
[1500405] = {320, 0, 1013},
[1500407] = {321, 0, 1012},
[1500411] = {322, 0, 2017},
[1500414] = {324, 0, 1012},
[1500419] = {327, 0, 1012},
[1500422] = {332, 0, 1013},
[1500423] = {331, 0, 2017},
[1500429] = {328, 0, 2017},
[1500430] = {281, 0, 5121},
[1600001] = {6, 0, 1006},
[1600002] = {7, 0, 1007},
[1600003] = {8, 0, 1008},
[1600004] = {9, 0, 1009},
[1600005] = {10, 0, 1010},
[1600006] = {11, 0, 1011},
[1600007] = {12, 0, 1012},
[1600008] = {13, 0, 1013},
[1600009] = {14, 0, 1014},
[1600010] = {15, 0, 1015},
[1600011] = {1, 0, 1001},
[1600012] = {2, 0, 1002},
[1600013] = {3, 0, 1003},
[1600014] = {4, 0, 1004},
[1600016] = {5, 0, 1005},
[1600017] = {39, 0, 2020},
[1600018] = {59, 0, 2021},
[1600019] = {75, 0, 2022},
[1600020] = {77, 0, 2010},
[1600021] = {78, 0, 2011},
[1600022] = {79, 0, 2012},
[1600023] = {80, 0, 2013},
[1600024] = {81, 0, 2014},
[1600025] = {82, 0, 2015},
[1600026] = {83, 0, 2016},
[1600027] = {84, 0, 2017},
[1600028] = {85, 0, 2018},
[1600029] = {86, 0, 2019},
[1600030] = {87, 0, 2001},
[1600031] = {88, 0, 2003},
[1600032] = {89, 0, 2002},
[1600033] = {90, 0, 2004},
[1600034] = {91, 0, 2005},
[1600035] = {92, 0, 2006},
[1600036] = {93, 0, 2007},
[1600037] = {94, 0, 2008},
[1600039] = {69, 0, 3020},
[1600040] = {54, 0, 3019},
[1600041] = {64, 0, 3021},
[1600042] = {76, 0, 3022},
[1600043] = {96, 0, 3009},
[1600044] = {97, 0, 3010},
[1600045] = {98, 0, 3011},
[1600046] = {99, 0, 3012},
[1600047] = {100, 0, 3013},
[1600048] = {101, 0, 3014},
[1600049] = {102, 0, 3016},
[1600050] = {103, 0, 3015},
[1600051] = {104, 0, 3017},
[1600052] = {105, 0, 3004},
[1600053] = {106, 0, 3007},
[1600054] = {107, 0, 3018},
[1600055] = {108, 0, 3006},
[1600056] = {109, 0, 3005},
[1600057] = {110, 0, 3002},
[1600058] = {111, 0, 3003},
[1600059] = {112, 0, 3001},
[1600064] = {235, 0, 2023},
[1600066] = {237, 0, 3023},
[1600075] = {245, 1, {5021,5022,5023,5024,5025,5026} },
[1600076] = {247, 1, {5027,5028,5029,5030,5031,5032} },
[1600077] = {248, 1, {5033,5034,5035,5036,5037,5038} },
[1600080] = {251, 1, {5051,5052,5053,5054,5055,5056} },
[1600081] = {255, 1, {5075,5076,5077,5078,5079,5080} },
[1600089] = {260, 1, {5105,5106,5107,5108,5109,5110} },
[1600092] = {263, 0, 2024},
[1600094] = {265, 0, 3024},
[1600100] = {281, 2, {5001, 5002, 5007, 5008} },
}


function init(npc)
    return false, false, 0, 0;  
end

function onEventStarted(player, npc, triggerName)

   -- require("/unique/".. npc.zone.zoneName .."/PopulaceShopSalesman/" .. npc:GetUniqueId()) 
    npcId = npc:GetActorClassId();

    if shopInfo[npcId] == nil then
        errorMsg = string.format("This PopulaceShopSalesman actor has no shop set. Actor Class Id: %s", npcId);
        player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "", errorMsg );
        player:EndEvent();
        return;
    end;
    
    shopCurrancy = 1000001;
    callClientFunction(player, "welcomeTalk", shopInfo[npcId][1], player);    

    if npcId == 1000159 then -- DoH Guild NPCs with Facility menu
        tutorialId = 36;
    elseif npcId == 1000163 then  
        tutorialId = 31;
    elseif npcId == 1001458 then
        tutorialId = 30;
    elseif npcId == 1600017 then
        tutorialId = 29;
    elseif npcId == 1600018 then
        tutorialId = 33;
    elseif npcId == 1600039 then
        tutorialId = 35;
    elseif npcId == 1600040 then
        tutorialId = 32;
    elseif npcId == 1600041 then
        tutorialId = 34;
    else
        tutorialId = -8;
    end


    
    while (true) do

        if (shopInfo[npcId][2] == 0) then
            choice = callClientFunction(player, "selectMode", tutorialId);
        elseif (shopInfo[npcId][2] == 1) then
            choice = callClientFunction(player, "selectModeOfClassVendor");
        elseif (shopInfo[npcId][2] == 2) then
            choice = callClientFunction(player, "selectModeOfMultiWeaponVendor", tutorialId);
        elseif (shopInfo[npcId][2] == 3) then
            choice = callClientFunction(player, "selectModeOfMultiArmorVendor", tutorialId);
        end
                
        if (choice == nil or choice == -3) then
            break;  
        end

        if (shopInfo[npcId][2] == 0) then
            processNormalShop(player, choice);
        elseif (shopInfo[npcId][2] == 1) then
            processMultiShop(player, choice);
        elseif (shopInfo[npcId][2] == 2) then
            processMultiShop(player, choice);
        elseif (shopInfo[npcId][2] == 3) then
            processMultiShop(player, choice);
        end
        
    end
    
    callClientFunction(player, "finishTalkTurn", player);
    player:EndEvent();
    
end

function processNormalShop(player, choice)
    if (choice == 1) then
        callClientFunction(player, "openShopBuy", player, shopInfo[npcId][3], shopCurrancy);
        
        while (true) do     
            buyResult, quantity = callClientFunction(player, "selectShopBuy", player);
            
            if (buyResult == 0) then
                callClientFunction(player, "closeShopBuy", player);                 
                break;
            else
              --  purchaseItem(player, shopInfo.shopContents[buyResult].id, quantity, shopInfo.shopContents[buyResult].hq, shopInfo.shopContents[buyResult].gil, shopInfo.shopCurrancy);  
            end
        end     
    elseif (choice == 2) then
        openSellMenu(player);
    elseif (choice == 3) then
        local classFacility = (shopInfo[npcId][1] + 1) or 35;
        facilityChoice = callClientFunction(player, "selectFacility", nil, classFacility, 3);
        
        if facilityChoice == 1 then 
            callClientFunction(player, "confirmUseFacility", player, 200);
        elseif facilityChoice == 2 then 
            callClientFunction(player, "confirmUseFacility", player, 400);
        elseif facilityChoice == 3 then 
            callClientFunction(player, "confirmUseFacility", player, 1000);           
        end
                    
    elseif (choice == 4) then
        callClientFunction(player, "startTutorial", nil, tutorialId);            
    end 
end

function processMultiShop(player, choice, sellType)

    if (choice >= 1 and choice <= 6) then
        callClientFunction(player, "openShopBuy", player, shopInfo[npcId][3][choice], shopCurrancy);
        
        while (true) do         
            buyResult, quantity = callClientFunction(player, "selectShopBuy", player);
            
            if (buyResult == 0) then
                callClientFunction(player, "closeShopBuy", player);                 
                break;
            else
              --  purchaseItem(player, shopInfo.shopContents[choice][buyResult].id, quantity, shopInfo.shopContents[choice][buyResult].hq, shopInfo.shopContents[choice][buyResult].gil, shopInfo.shopCurrancy);
            end
        end     
    elseif (choice == 0) then
        openSellMenu(player);
    elseif (choice == 6) then
        callClientFunction(player, "selectFacility", 2, 35, 3);
        callClientFunction(player, "confirmUseFacility", player, 35);           
    elseif (choice == 7) then
        callClientFunction(player, "startTutorial", nil, tutorialId);            
    end
    
end


function openSellMenu(player)
    callClientFunction(player, "openShopSell", player);

    while (true) do     
        sellResult, sellQuantity, sellState, unknown, sellItemSlot = callClientFunction(player, "selectShopSell", player);
            
        if (sellResult == nil) then
            callClientFunction(player, "closeShopSell", player);                    
            break;
        else
            if sellState == 1 then
                itemToSell = player:GetItemPackage(INVENTORY_NORMAL):GetItemAtSlot(sellItemSlot-1);
                gItemSellId = itemToSell.itemId; 
                gItemQuality = itemToSell.quality;
                gItemPrice = GetItemGamedata(gItemSellId);
                gItemPrice = gItemPrice.sellPrice;
                
                
                if gItemQuality == 2 then       -- +1
                    gItemPrice = (math.floor(gItemPrice * 1.10));
                elseif gItemQuality == 3 then   -- +2
                    gItemPrice = (math.floor(gItemPrice * 1.25));
                elseif gItemQuality == 4 then   -- +3
                    gItemPrice = (math.floor(gItemPrice * 1.50));
                end

                callClientFunction(player, "informSellPrice", 1, sellItemSlot, gItemPrice);

            elseif sellState == nil then
                sellItem(player, gItemSellId, sellQuantity, gItemQuality, gItemPrice, sellItemSlot-1, shopCurrancy);
            end
        end
    end
end

