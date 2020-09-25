--[[

HarvestJudge 

Operates the harvesting system for mining, logging, and fishing.

Functions:

loadTextData()
    Desc: Loads all gamesheets needed and instantiates a HarvestJudge.
	Params: None
        
targetCancel()
    Desc: Cancels the player's target.
    Params: None
    
turnToTarget()
    Desc: Turns to a direction
    Params: * harvestType           - Harvest command used.  Client script has a _waitForTurning() for Quarry/Harvest/Spearfishing
            * direction             - The pi radian to turn the character towards, server has to calculate the vector between the actors.
    
openInputWidget()
    Desc: Inits the widget system (call first).
    Params: * harvestType           - Determines which text strings to load based on the harvestType
            * nodeGrade             - The grade of the node.  Retail went up to grade 5.

orderInputWidget()
    Desc: Updates the node HP.
    Params: * nodeRemainder         - Range goes from 0-100
            * unk1                  -
            * harvestType           - Doesn't appear to visually do anything? Script checks against harvest command id
    
textInputWidget()
    Desc: Sets the result text after a minigame is performed.
    Params: * harvestType           - The harvest command
            * unk1                  - Actor to grab text from?  Set to the harvestJudge so the rest of params function, otherwise widget prints whatever is set here.
            * textId                - Id from the harvestJudge sheet.
            * textIdParam1          - Used to fill in textId details if the sheet requires it, Eg. textId #25 requires an itemId, HQ quality, and yield filled in.
            * textIdParam2          
            * textIdParam3          
            * commandId             - If textId = nil, client script sets it to 64 and this parameter is assigned to Param1
                                    - Why does this exist?  Setting textId to 64 and using commandId as textIdParam1 does the same job.
            
askInputWidget()
    Desc: Gets user input after opening a ask widget.  Returns two values, one being the id of the chosen command, and the "currentPower" of the minigame.
    Params: * harvestType           - The harvest command
            * phase                 - The current minigame window to show.  Valid ids 1 & 2.
            * showTutorial          - Shows Tutorial menu option in the window if not = 0.
            * showFishWait          -
            * showFishWaitAndJig    -
            * updateFishHP          -
            * showRareCatalystEffect-
 
closeInputWidget()
    Desc: Closes the widget system (call last).
    Params: * harvestType           - The harvest command

rangeInputWidget()
    Desc: Unknown, currently errors the client...
    Params: * harvestType
            * phase 
            * goodMin
            * goodMax
            * bool 
            
            
Notes:
            * Aim = Where on the aim gauge the player chose.
            * Remainder = How many attempts you get on the section portion of the minigame
            * Sweetspot = Where you hit on the second portion of the minigame
--]]

minerAnim = {0x14001000, 0x14002000, 0x14003000};

--[[ Mooglebox - Aim
+5  +4  +3  +2  +1   0  -1  -2  -3  -4   -5
 0  10  20  30  40  50  60  70  80  90  100

Sweetspots   1=10  2=30  3=70  4=100 for Mining
Remainder    A=40  B=60  C=70  D=80
--]]


harvestNodeContainer = { -- nodeGrade, harvestAttempts, #ofItemsBecauseICantIntoIpairs, Item1, Item2, etc
    [1001] = {2, 2, 3, 1, 2, 3},
    [1002] = {2, 4, 5, 3005, 3003, 3002, 3001, 3004}
}

harvestNodeItems = { 
        --itemId, remainder, aim, sweetspot, max yield
    [1] = {10009104, 70, 30, 30, 4}, -- Rock Salt
    [2] = {10006001, 80, 10, 30, 4}, -- Bone Chip
    [3] = {10001006, 80, 20, 30, 3},  -- Copper Ore
    [3001] = {10001003, 80, 50, 30, 3},
    [3002] = {10001006, 70, 70, 10, 4},
    [3003] = {10001005, 80, 90, 70, 1},
    [3004] = {10009104, 40, 10, 100, 2},
    [3005] = {10001007, 40,  0, 30, 1}
   
}


require ("global")

function onEventStarted(player, commandActor, triggerName, arg1, arg2, arg3, arg4, checkedActorId)

    debugMsg = false;
    
    powerCurrent = 0;
    powerLast = 0;
    powerRange = 10;    -- 'Feels' look a good amount compared to vids/ARR's minigame.
    
    showTutorial = 0;
    
    commandMine = 22002;
    commandLog = 22003;
    commandFish = 22004;

    harvestNodeId = 1001; -- What the server should send eventually
    harvestNode = BuildHarvestNode(player, harvestNodeId);  -- [1-11] = {itemId, remainder, sweetspot, maxYield}
    harvestGrade = harvestNodeContainer[harvestNodeId][1] or 0;
    harvestAttempts = harvestNodeContainer[harvestNodeId][2] or 0;
    nodeRemainder = 0;

       
    
    harvestType = commandMine;
    
    worldMaster = GetWorldMaster();
    harvestJudge = GetStaticActor("HarvestJudge");
    callClientFunction(player, "delegateCommand", harvestJudge, "loadTextData", commandActor);
    --callClientFunction(player, "delegateCommand", harvestJudge, "targetCancel", commandActor);
    --callClientFunction(player, "delegateCommand", harvestJudge, "turnToTarget", commandActor, harvestType, nodeGrade);

    player:ChangeState(50);
	

    
    if harvestType == commandMine then
        player:SendGameMessage(harvestJudge, 26, MESSAGE_TYPE_SYSTEM, 1, harvestGrade);
        
        callClientFunction(player, "delegateCommand", harvestJudge, "openInputWidget", commandActor, harvestType, harvestGrade);
        callClientFunction(player, "delegateCommand", harvestJudge, "orderInputWidget", commandActor, nodeRemainder, nil, harvestType);
        callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, nil, 0, 0, 0, 0);

        
        
        while harvestAttempts > 0 do
        
            -- "Aim", 0 = Top of bar, 100 = Bottom.
            menuResult, sliderPhase, unk3 = callClientFunction(player, "delegateCommand", harvestJudge, "askInputWidget", commandActor, harvestType, 1, showTutorial, false, false, nil, false); 
            if debugMsg then player:SendMessage(0x20, "", "menuResult: "..tostring(menuResult).." sliderPhase: "..tostring(sliderPhase).." Unk: "..tostring(unk3)); end
        
            if menuResult == 22701 then     -- Begin.

                local aimSlot = (sliderPhase/10)+1; -- Thanks LUA index = 1
                
                local nodeDetails = harvestNode[aimSlot];
                local nodeItem = nodeDetails[1];
                local nodeRemainder = nodeDetails[2];
                local nodeSweetspot = nodeDetails[3];
                local nodeYield = nodeDetails[4];
                local isFirstSwing = true;
                
                local sweetspotDifference;
                local sweetspotDifferencePrevious;
                
                if debugMsg then 
                    player:SendMessage(0x20, "", "aimSlot: "..(aimSlot).."   itemId:"..tostring(nodeDetails[1]).." remainder: "..tostring(nodeDetails[2]));
                end
        
                
                player:SendGameMessage(harvestJudge, 36, MESSAGE_TYPE_SYSTEM); -- "You set your sights on an area."


                
                callClientFunction(player, "delegateCommand", harvestJudge, "orderInputWidget", commandActor, nodeRemainder, nil, harvestType);
                
                while true do
                    callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, nil, 0, 0, 0, 0);
                    
                    -- "Strike" 0 = Empty, 100 = Filled.   Mooglebox sweespots are 1=10, 2=30, 3=70, 4=100 for Mining
                    chosenCommand, powerCurrent = callClientFunction(player, "delegateCommand", harvestJudge, "askInputWidget", commandActor, harvestType, 2, showTutorial, false, false, nil, false); -- Strike
    
                    if debugMsg then player:SendMessage(0x20, "", tostring(chosenCommand).." Power: "..tostring(powerCurrent)); end
                    
                    
                    if chosenCommand == 22702 then      -- Cancel.
                        harvestAttempts = harvestAttempts - 1;
                        
                        if harvestAttempts > 0 then
                            -- You can make # more gathering attempts.
                            player:SendGameMessage(player, worldMaster, 40344, 0x20, harvestAttempts);
                        else
                            -- There is nothing left to gather at this location.
                            player:SendGameMessage(player, worldMaster, 40339, 0x20, harvestAttempts);
                        end
                        break;
                        
                    elseif chosenCommand == 22703 then  -- Strike.
                        
                        
                        nodeRemainder = nodeRemainder - 20;
                        if nodeRemainder < 0 then 
                            nodeRemainder = 0;  
                        end
                        
                        callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, nil, 0, 0, 0, 0);
                        
                        player:PlayAnimation(minerAnim[math.random(1,3)]);
                        wait(2);
                        sweetspotDifference = math.abs(powerCurrent - nodeSweetspot);
                        
                        
                        if powerRange >= sweetspotDifference then
                            callClientFunction(player, "delegateCommand", harvestJudge, "orderInputWidget", commandActor, nodeRemainder, false, harvestType);
                            
                            -- "You obtain <yield> <item> <quality>"
                            callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, 25, nodeItem, 0, nodeYield, 0);
                            
                            player:SendGameMessage(player, worldMaster, 40301, MESSAGE_TYPE_SYSTEM, player, nodeItem, nodeYield); -- TODO: Refer to caps to see wtf is going on here
                            
                            
                            HarvestReward(player, nodeItem, nodeYield);
                            nodeRemainder = 0;
                        else
                            if isFirstSwing then
                                if sweetspotDifference < 19 then  -- TODO: TWEAK THESE, likely need to be larger
                                    callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, 45);
                                    player:SendGameMessage(harvestJudge, 45, MESSAGE_TYPE_SYSTEM); -- "You feel something promising."
                                elseif sweetspotDifference > 20 then
                                    callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, 42);
                                    player:SendGameMessage(harvestJudge, 42, MESSAGE_TYPE_SYSTEM); -- "You feel nothing promising."
                                end
                            else
                                if sweetspotDifference > sweetspotDifferencePrevious then
                                    callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, 43);
                                    player:SendGameMessage(harvestJudge, 43, MESSAGE_TYPE_SYSTEM); -- "You are getting farther from the mark."
                         
                                elseif  sweetspotDifference < sweetspotDifferencePrevious then
                                    callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, 44);
                                    player:SendGameMessage(harvestJudge, 44, MESSAGE_TYPE_SYSTEM); -- "You are getting closer to the mark."   
                                else
                                    callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandActor, harvestType, harvestJudge, 42);
                                    player:SendGameMessage(harvestJudge, 42, MESSAGE_TYPE_SYSTEM); -- "You feel nothing promising."  
                                end
                            end       
                        end
                        
                        if not isFirstSwing then
                            powerLast = powerCurrent;
                        end;

                        callClientFunction(player, "delegateCommand", harvestJudge, "orderInputWidget", commandActor, nodeRemainder, false, harvestType);
                        

                        if nodeRemainder == 0 then
                            harvestAttempts = harvestAttempts - 1;
                            
                            if harvestAttempts > 0 then
                                -- You can make # more gathering attempts.
                                player:SendGameMessage(player, worldMaster, 40344, 0x20, harvestAttempts);
                            else
                                -- There is nothing left to gather at this location.
                                player:ChangeMusic(101);
                                player:SendGameMessage(player, worldMaster, 40339, 0x20, harvestAttempts);
                            end  
                        
                            wait(2);
                            break;
                        end
                        
                        
                        if isFirstSwing and debugMsg then player:SendMessage(0x20, "", "First swing"); end
                        
                        isFirstSwing = false;
                        sweetspotDifferencePrevious = sweetspotDifference; 
                        
                    elseif chosenCommand == 22710 then -- "Strike" Tutorial.                    
                        SendTutorial(player, harvestJudge, 2);
                    end
                    
                end
                
            elseif menuResult == 22702 then -- Cancel.
                break;
            elseif menuResult == 22710 then -- "Aim" Tutorial.
                SendTutorial(player, harvestJudge, 1);
            end
        end
        
    elseif harvestType == commandLog then
    
    elseif harvestType == commandFish then
    
    end
    
    player:SendGameMessage(harvestJudge, 31, MESSAGE_TYPE_SYSTEM);
    
    
    if harvestAttempts == 0 then
        player:SendGameMessage(player, worldMaster, 40310, 0x20); -- "The deposit has been exhausted."
        --TO:DO Despawn node + whatever logic to respawn an exsiting expired node in the area.
    end
     
	callClientFunction(player, "delegateCommand", harvestJudge, "closeInputWidget", commandActor, harvestType);
    
	
    player:ChangeState(0);
	player:EndEvent();
	
end



-- Returns a table in the following format:  nodeTable = { [1-11] = {itemId, remainder, sweetspot, maxYield} }
function BuildHarvestNode(player, sentNode)

    if harvestNodeContainer[sentNode] then
        local node = harvestNodeContainer[sentNode];
        local nodeTable = {};
        local nodeItems = {};
        local nodeItemCount = node[3];
        
        local grade = node[1];
        local attempts = node[2];
        
   
        -- Load up nodeItems[] with the harvestNodeItems{} key and Aim point
        for i=1, nodeItemCount do
            local nodeItemKey = node[3+i];
            local item = harvestNodeItems[ node[3+i] ]
            
            nodeItems[i] = { nodeItemKey, ((item[3] / 10)+1) };
            
            if debugMsg then  player:SendMessage(0x20, "", "nodeItems: "..nodeItems[i][1].." "..nodeItems[i][2]); end
        end
    
        -- Iterate through the 11 Aim spots
        for i=1,11,1 do 
            local hasItem = false;
            
            -- See if there's a nodeItems[] that has an Aim spot that matches the current loop
            -- TODO: Just set nodeItems[] keys to the actual slots to skip this loop inside a loop
            for j=1, nodeItemCount do
                if nodeItems[j][2] == i then
                    hasItem = j;
                    break;
                end
            end
         
            if hasItem then
                local item = harvestNodeItems[ nodeItems[hasItem][1] ];
                
                -- Assign itemId, remainder, sweetspot, yield to this slot
                nodeTable[i] = {item[1], item[2], item[4], item[5] };
                
                if debugMsg then 
                    player:SendMessage(0x20, "", "nodeTable: "..i.." "..nodeTable[i][1].." "..nodeTable[i][2].." "..nodeTable[i][3].." "..nodeTable[i][3]); 
                end
                
            else
                nodeTable[i] = {0,0,0,0};
                if debugMsg then player:SendMessage(0x20, "", "nodeTable: "..i); end
            end                        
        end
        
        return nodeTable
    end
    
end



function SendTutorial(player, harvestJudge, id)
    
    if id == 1 then
        player:SendGameMessage(harvestJudge, 1, MESSAGE_TYPE_SYSTEM);
        wait(3);
        player:SendGameMessage(harvestJudge, 4, MESSAGE_TYPE_SYSTEM);
    elseif id == 2 then
        player:SendGameMessage(harvestJudge, 7, MESSAGE_TYPE_SYSTEM);
        wait(3);
        player:SendGameMessage(harvestJudge, 10, MESSAGE_TYPE_SYSTEM);                  
        wait(3);
        player:SendGameMessage(harvestJudge, 13, MESSAGE_TYPE_SYSTEM); 
        wait(3);
        player:SendGameMessage(harvestJudge, 16, MESSAGE_TYPE_SYSTEM);  
    end

end

function HarvestReward(player, item, qty)  -- Really should get a helper function for this

    local worldMaster = GetWorldMaster();
    local location = INVENTORY_NORMAL;
    local invCheck = player:getItemPackage(location):addItem(item, qty, 1);       
    
    if (invCheck == INV_ERROR_FULL) then
        -- Your inventory is full.
        player:SendGameMessage(player, worldMaster, 60022, MESSAGE_TYPE_SYSTEM_ERROR);
    elseif (invCheck == INV_ERROR_ALREADY_HAS_UNIQUE) then
        -- You cannot have more than one <itemId> <quality> in your possession at any given time.
        player:SendGameMessage(player, worldMaster, 40279, MESSAGE_TYPE_SYSTEM_ERROR, item, 1);
    elseif (invCheck == INV_ERROR_SYSTEM_ERROR) then
        player:SendMessage(MESSAGE_TYPE_SYSTEM_ERROR, "", "[DEBUG] Server Error on adding item.");
    elseif (invCheck == INV_ERROR_SUCCESS) then
        --player:SendMessage(MESSAGE_TYPE_SYSTEM, "", message);
        player:SendGameMessage(player, worldMaster, 25246, MESSAGE_TYPE_SYSTEM, item, qty);
    end
end