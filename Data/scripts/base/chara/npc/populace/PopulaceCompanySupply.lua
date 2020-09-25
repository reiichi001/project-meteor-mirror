--[[

PopulaceCompanySupply Script

This class handles the menus for player's delivering specific items in exchange for grand company seals.  
The supply/provision schedule runs on a weekly rotation, which resets Monday at 12AM JST, with eight rotations total to cycle through.
Each desired item has a server-wide max that it can be turned in, and when that is fulfilled, it moves to the next item in that week's list to work on.

NPCs involved in the script use the Noc001 script for dialog and menu interactions.


Functions:

eventTalkPreJoin()                              - Dialog when you're not affiliated
eventTalkExclusive()                            - Dialog when you're part of a GC but not the one of the actor?
eventTalkJoined()                               - Salutes then softlocks the client due to removed dialog strings. Obsolete function.

eventQuestItemMenuOpen(itemId, itemPrice, itemPriceHq, supplyType) - supplyType: 1 = Supply, 2 = Provisioning,  3 = Totorak, 4 = Dzmael, 5 = Primal, 6 = NM drops
eventQuestItemMenuSelect(quantity, quality, unk) - Brings up the shop-style menu for viewing item detail and confirming item delivery.  Args appear to do nothing on client?
eventQuestItemMenuClose()                                    - Closes menu

eventQuestSupplyItemActor(unk1)     -- Client calls this automatically for setting up Expeditionary window in some manner
eventQuestSupplyItemID(unk1, unk2)  -- eventQuestSupplyItemActor() calls this to sets item ranges based on category

getEventQuestSupplyMode()                       - Returns current supply mode set by eventQuestItemMenuOpen()
eventTalkStepBreak()                            - Resets actor engage state


Noc001 Functions:

pENPCAskSupplyWelcome(npcGC)  -- Welcome dialog
pENPCAskSupply(npcGC)         -- Brings up the delivery selection menu
eventQuestAskExWelcome(npcGC) -- Dialog when you pick Expeditionary
eventQuestAskExArea(npcGC)    -- Brings up the Expeditionary selection menu
pENPCAskNowTalk(npcGC)        -- Dialog for picking Delivery Status from pENPCAskSupply()

nowSup(itemId1, current1, max1, itemId2, current2, max2, itemId3, current3, max3) -- Says current 3 items and current amount delivered vs. max it'll take
nowSupAddItem(itemId, current, max)                                               -- Lists bonus item
pItem(itemId1, unk1, itemId2, unk2, itemId3, unk3, itemId4, unk4) -- Lists which item(s) you want to delivery. Fourth item is the bonus, set 0 for hidden.

showSupplyLimit(minutes, seconds, current, required)   -- Shows time remaining to finish delivery, shows current/required amount
eventShowPrizeMessage(npcGC)                           -- Reward dialog for handing something in?

pELimitErr()                                           -- Error msg for GC no longer accepting items. 
pETradeErr()                                           -- Transaction error.  Inventory error?
pETradeErrLimit(minutes, seconds, current, required)   -- Transaction error.  Shows time remaining and current/required amount
pESuppylMaxErrKeyWait(isShowLimit, minutes, seconds, current, required) -- Error msg for delivery quota already filled.  Optional timer/amount display
pESuppylSealMaxErr()                                                    -- Error msg for capped on GC seals, transaction incomplete

eventQuestCantEx(npcGC)      -- Dialog explaining you need to be Private Second Class to do Expeditionary missions
--]]

require ("global")
require ("shop")

function init(npc)
    return false, false, 0, 0;  
end

local gcRep = { 
    [1500210] = 1, -- Maelstrom Representative
    [1500211] = 2, -- Adder Representative
    [1500212] = 3, -- Flame Representative
}

local gcItems = { -- Debug purposes.  Static item list with seal value and max turn-in.
    [111] = {id = 10002015, seals = 8, cap = 1900},
    [112] = {id = 8031419, seals = 68, cap = 300},
    [113] = {id = 3010011, seals = 3, cap = 5000},
    [114] = {id = 8011108, seals = 89, cap = 400},

    [115] = {id = 10004001, seals = 5, cap = 3000},
    [116] = {id = 10008109, seals = 3, cap = 5000},
    [117] = {id = 12000180, seals = 5, cap = 3000},
    [118] = {id = 10004026, seals = 9, cap = 3400},

    [121] = {id = 10008211, seals = 5, cap = 3000},
    [122] = {id = 3020407, seals = 5, cap = 2500},
    [123] = {id = 8030220, seals = 92, cap = 200},
    [124] = {id = 8030922, seals = 99, cap = 400},

    [125] = {id = 10001014, seals = 3, cap = 5000},
    [126] = {id = 10008007, seals = 5, cap = 3000},
    [127] = {id = 3011217, seals = 3, cap = 5000},
    [128] = {id = 3011207, seals = 3, cap = 6000},

    [131] = {id = 4030204, seals = 69, cap = 300},
    [132] = {id = 10004103, seals = 9, cap = 1700},
    [133] = {id = 10009208, seals = 6, cap = 3000},
    [134] = {id = 1, seals = 1, cap = 1}, -- Unknown

    [135] = {id = 10004008, seals = 9, cap = 1700},
    [136] = {id = 10008007, seals = 5, cap = 3000},
    [137] = {id = 3011201, seals = 5, cap = 3000},
    [138] = {id = 10009401, seals = 6, cap = 6000},

    [211] = {id = 10002012, seals = 5, cap = 3000},
    [212] = {id = 4100007, seals = 51, cap = 300},
    [213] = {id = 3010108, seals = 2, cap = 3000},
    [214] = {id = 8080825, seals = 42, cap = 800},

    [215] = {id = 10004003, seals = 5, cap = 3000},
    [216] = {id = 10002012, seals = 3, cap = 5000},
    [217] = {id = 3011104, seals = 2, cap = 3000},
    [218] = {id = 3011107, seals = 3, cap = 6000},

} 


local gcWeek = {     -- Debug purposes. Static weekly item lists.  [week] = { [city] =  {[category] = { info } } }
    [1] = {
        [1] = { -- Limsa
            [1] = { -- Supply
                gcItems[111],
                gcItems[112],
                gcItems[113],
                gcItems[114],
            },
            [2] = { -- Provision
                gcItems[115],
                gcItems[116],
                gcItems[117],
                gcItems[118],       
            }
        },
        [2] = { -- Gridania
            [1] = { -- Supply
                gcItems[121],
                gcItems[122],
                gcItems[123],
                gcItems[124],
            },
            [2] = { -- Provision
                gcItems[125],
                gcItems[126],
                gcItems[127],
                gcItems[128],       
            }
        },
        [3] = { -- Ul'dah
            [1] = { -- Supply
                gcItems[131],
                gcItems[132],
                gcItems[133],
                gcItems[134],
            },
            [2] = { -- Provision
                gcItems[135],
                gcItems[136],
                gcItems[137],
                gcItems[138],       
            }
        }
    },
        
    [2] = {
        [1] = { -- Limsa
            [1] = { -- Supply
                gcItems[211],
                gcItems[212],
                gcItems[213],
                gcItems[214],
            },
            [2] = { -- Provision
                gcItems[215],
                gcItems[216],
                gcItems[217],
                gcItems[218],       
            }
        }
    }

}

local gcDelivery  = { -- Debug purposes.  Holds values for current turned in amount and 4th item bonus status.
    week = 1,
    currentCount = {
        {
            {49, 81, 5000, 5}, {2402, 4779, 589, 2}     -- Limsa Supply/Provision
        },  
        {
            {1, 2, 3, 4}, {5, 6, 7, 8}                  -- Gridania Supply/Provision
        },
        {
            {10, 32, 9, 18}, {23, 49, 9, 300}           -- Ul'dah Supply/Provision
        }
    },    
    bonus = { {1, 1}, {0,1}, {0,1} }; -- City -> {Supply, Provision}
    timeRemainingMinutes = 99,
    timeRemainingSeconds = 59,
}

local supplyQuest = GetStaticActor("Noc001");
local skipGCcheck = false;    -- Debug 
local skipRankCheck = false;  -- Debug
local gcCheckProceed = false; -- Debug




function onEventStarted(player, npc, triggerName)
    local playerGC = player.gcCurrent;
    local limsaRank = player.gcRankLimsa;
    local gridaniaRank = player.gcRankGridania;
    local uldahRank = player.gcRankUldah;
    local playerGCSeal = 1000200 + playerGC;
    
    local npcId = npc:GetActorClassId();
    local npcGC = gcRep[npcId]; 
    
    if (skipGCcheck == true) then
        gcCheckProceed = true;
    end
        
    if ((playerGC ~= npcGC) and skipGCcheck == false)  then
        if (playerGC == 0) then
            callClientFunction(player, "eventTalkPreJoin");
        else
            callClientFunction(player, "eventTalkExclusive");
        end
    else
        gcCheckProceed = true;
    end
    
    if gcCheckProceed then
        callClientFunction(player, "delegateEvent", player, supplyQuest, "pENPCAskSupplyWelcome", gcRep[npcId]);
        while (true) do
        
            local choice = callClientFunction(player, "delegateEvent", player, supplyQuest, "pENPCAskSupply", gcRep[npcId]);
            
            if (choice == 2) then -- Supply
                deliveryMenuInfo(player, npcGC, 1);
                
            elseif (choice == 3) then -- Provision
                deliveryMenuInfo(player, npcGC, 2);

            elseif (choice == 4) then -- Expeditionary
                local proceed = false;
                
                if (skipRankCheck == true) then
                    proceed = true;
                else
                    if (playerGC == 1 and limsaRank >= 13 and limsaRank <= 111) 
                    or (playerGC == 2 and gridaniaRank >= 13 and gridaniaRank <= 111) 
                    or (playerGC == 3 and uldahRank >= 13 and uldahRank <= 111) then
                        proceed = true
                    end
                end

                if proceed == true then
                    callClientFunction(player, "delegateEvent", player, supplyQuest, "eventQuestAskExWelcome", gcRep[npcId]); 
                    while (true) do
                        local exChoice = callClientFunction(player, "delegateEvent", player, supplyQuest, "eventQuestAskExArea", gcRep[npcId]); 
                        
                        if (exChoice >= 3) then
                            deliveryMenuOpen(player, npc, 0,0,0, exChoice);
                        else
                            break;
                        end
                    end
                else
                    callClientFunction(player, "delegateEvent", player, supplyQuest, "eventQuestCantEx",gcRep[npcId]);
                end

            elseif (choice == 5) then -- Requested item
                deliveryStatus(player, npcGC);
            else
                break;
            end
            
            wait(1);
        end
    end

    callClientFunction(player, "eventTalkStepBreak"); 
    player:endEvent()

end


function deliveryMenuInfo(player, city, category)

    local gcContents = getWeeklyItems(city, category);
    local gcCurrent = getCurrentCount(city, category);
    local supplyChoice = 0;
    
    while (true) do 
    
        if gcDelivery.bonus[city][category] == 1 then     -- Show fourth item if condition is met, otherwise show three.
            
            supplyChoice = callClientFunction
            (
                player, 
                "delegateEvent", 
                player, 
                supplyQuest, 
                "pItem", 
                gcContents[1].id, 
                1, 
                gcContents[2].id, 
                1, 
                gcContents[3].id, 
                1, 
                gcContents[4].id, 
                1
            );
        else
            supplyChoice = callClientFunction
            (
                player, 
                "delegateEvent", 
                player, 
                supplyQuest, 
                "pItem", 
                gcContents[1].id, 
                1, 
                gcContents[2].id, 
                1, 
                gcContents[3].id,  
                1, 
                0, 
                0
            );
        end
        
        if supplyChoice >= 2 then
        
            if gcCurrent[supplyChoice-1] < gcContents[supplyChoice-1].cap then
                local hqPrice = math.ceil(gcContents[supplyChoice-1].seals * 1.5);
                
                deliveryMenuOpen
                (
                    player, 
                    npc, 
                    gcContents[supplyChoice-1].id, 
                    gcContents[supplyChoice-1].seals, 
                    hqPrice, 
                    category
                );
                
            else
                callClientFunction(player, "delegateEvent", player, supplyQuest, "pESuppylMaxErrKeyWait");
            end
        elseif supplyChoice == 1 then
            break;
        end
        wait(1);
    end
end


function deliveryMenuOpen(player, npc, itemId, price, hqPrice, supplyType)

    callClientFunction(player, "eventQuestItemMenuOpen", itemId, price, hqPrice, supplyType);

    while (true) do
    
        local choice, quantity, quality, itemSlot, Type7Param = callClientFunction(player, "eventQuestItemMenuSelect");
          
        if choice == false then
            callClientFunction(player, "eventQuestItemMenuClose");
            break;
        end
        
        --[[
        player:SendMessage(0x20, "", "Choice: " .. tostring(choice));
        player:SendMessage(0x20, "", "Quantity: " .. tostring(quantity));
        player:SendMessage(0x20, "", "Quality: " .. tostring(quality));
        player:SendMessage(0x20, "", "Slot: " .. tostring(itemSlot));   -- Broke at some point, always return 0, investigate sometime
        player:SendMessage(0x20, "", "Type7Param: " .. tostring(Type7Param.slot));
        --]]
        
        pickedItem = GetItemGamedata(player:GetItemPackage(INVENTORY_NORMAL):GetItemAtSlot(Type7Param.slot).itemId).name;
        player:SendMessage(0x20, "", "Player tried to deliver " .. quantity .. " " ..  pickedItem);
        
        -- TODO: Add error handling for capped seals, no-long-available-to-deliver, etc
        wait(1);
    end
end



function deliveryStatus(player, city)
    local gcContents = getWeeklyItems(city, 1);
    local gcCurrent = getCurrentCount(city, 1);

    callClientFunction(player, "delegateEvent", player, supplyQuest, "pENPCAskNowTalk", gcRep[npcId]);
    callClientFunction
    (
        player, 
        "delegateEvent", 
        player, 
        supplyQuest, 
        "nowSup", 
        gcContents[1].id, 
        gcCurrent[1], 
        gcContents[1].cap,
        gcContents[2].id, 
        gcCurrent[2], 
        gcContents[2].cap,
        gcContents[3].id, 
        gcCurrent[3], 
        gcContents[3].cap
    );
    if gcDelivery.bonus[city][1] == 1 then
        callClientFunction
        (
            player, 
            "delegateEvent", 
            player, 
            supplyQuest, 
            "nowSupAddItem", 
            gcContents[4].id, 
            gcCurrent[4], 
            gcContents[4].cap
        );
    end;
    
    gcContents = getWeeklyItems(city, 2);
    gcCurrent = getCurrentCount(city, 2);
    
    callClientFunction
    (
        player, 
        "delegateEvent", 
        player, 
        supplyQuest, 
        "nowSup", 
        gcContents[1].id, 
        gcCurrent[1], 
        gcContents[1].cap,
        gcContents[2].id, 
        gcCurrent[2], 
        gcContents[2].cap,
        gcContents[3].id, 
        gcCurrent[3], 
        gcContents[3].cap
    );
    if gcDelivery.bonus[city][2] == 1 then
        callClientFunction
        (
            player, 
            "delegateEvent", 
            player, 
            supplyQuest, 
            "nowSupAddItem", 
            gcContents[4].id, 
            gcCurrent[4], 
            gcContents[4].cap
        );
    end;
   
    callClientFunction(player, "delegateEvent", player, supplyQuest, "showSupplyLimit", gcDelivery.timeRemainingMinutes, gcDelivery.timeRemainingSeconds, 2, 8);
end


function getWeeklyItems(city, category)
   return gcWeek[gcDelivery.week][city][category]
end

function getCurrentCount(city, category)
    return gcDelivery.currentCount[city][category];
end

