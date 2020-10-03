--[[

CraftJudge 

Operates the Crafting system.

Functions:

loadTextData()  
	Desc: Loads all gamesheets needed and instantiates a CraftJudge.
	Params: None

start(facility, requestsMode, material1, material2, material3, material4, material5, material6, material7, material8)
    Desc: Opens the Craft Start widget, with any preloaded materials. Widget has two modes; one for normal synthesis and another
          for local leve "requested items" mode.
    Params: * facility/widgetMode   - The current facility id buff the player may have. After opening a recipe tab, start() has to be called with this
                                      set to -1. After the player chooses a recipe, start() has to be called with this set to -2.
            * requestMode           - If true, switches the UI to Requested Items mode otherwise it opens Normal Synthesis mode.
            * material1-8           - ItemID for each of the 8 material slots. If empty, they must be set to 0 or the client will crash.
		
closeCraftStartWidget()
	Desc: Closes the Craft Start widget.
	Params: None

selectRcp(item1, item2, item3, item4, item5, item6, item7, item8)
    Desc:     Opens a recipe selection window. If one recipe is provided, automatically selects that recipe.
    Params:    * itemId1-8                - The itemIDs to show in the list. If only one provided, select it.
	
confirmRcp(craftedItem, quantity, crystalItem1, crystalQuantity1, crystalQuantity1, crystalItem2, crystalQuantity2, recommendedSkill, recommendedFacility)
    Desc: Opens the confirmation window, detailing what is needed and the item that will be created. Requires a selectRcp() call first.
    Params: * craftedItem           - The itemID of the item to be crafted.
            * quantity              - Quantity of crafted items.
            * crystalItem1          - The first required crystal itemID for crafting.
            * crystalQuantity1      - Quantity of the first crystal.
            * crystalItem2          - The second required crystal itemID for crafting.
            * crystalQuantity2      - Quantity of the second crystal.
            * recommendedSkill      - Which itemID to display under the "Recommended Skill" panel.
            * recommendedFacility   - Which facility to display under the "Recommended Facility" panel.

selectCraftQuest()
	Desc: Opens the journal to select the local leve that the player would like to do.
	Params: None

confirmLeve()
	Desc: Opens the summery page for the local leve.
	Params: * localLeveID			-
			* craftedItem			-
			* ?
			* ?
			* itemsCompleted		-
			* remainingMaterials	-
			* ?
			* ?

askContinueLocalLeve(localLeveID, craftedItem, itemsCompleted, craftTotal, attempts)
	Desc: Opens the dialog to continue crafting for a local leve after an item was completed.
	Params: * localLeveID			- The id of the current leve in progress.
			* craftedItem			- The current crafted item id.
			* itemsCompleted		- Number of items crafted so far.
			* craftTotal			- Number of items to be crafted in total.
			* attempts				- The number of attempts left.

askRetryLocalleve(localLeveID, allowanceCount)
	Desc: Opens the dialog to retry the local leve (at the expense of an allowance) if the player had failed it.
	Params: * localLeveID			- The failed level id.
			* allowanceCount		- How many allowances the player has.

openCraftProgressWidget(durability, quality, hqChance)
	Desc: Opens the crafting minigame, sets starting values.
	Params: * durability			- Durability of the current item.
			* quality				- Starting quality of the current item.
			* hqChance				- Starting chance to get a HQ item.

craftCommandUI(classID, hasWait, command1, command2, command3, command4, command5)
	Desc: Sets the available command list and waits for the player to select a command.
	Params:
			* classID				- The current crafting class. Must be set properly to show the three synthesis commands.
			* hasWait				- If true, adds the wait command.
			* command1-5			- Five possible crafting commands (crafting skills).

craftTuningUI(command1, command2, command3, command4, command5, command6, command7, command8)
	Desc: Displays only the provided  commands for the "Double Down" phase that happens after crafting.
	Params: * command1-8			- The list of commands available.

updateInfo(progress, durability, quality, tuningItem, tuningItemQuality, tuningItemQuantity, hqChance)
	Desc: Updates the progress UI components and text boxes.
	Params: * progress				- The current crafting progress percentage. Value is from 0 to 100.
			* durability			- The current durability of the crafted item.
			* quality				- The current quality of the crafted item.
			* tuningItem			- The crafted item to show in the Tuning UI. Nil if crafting. Deprecated in 1.23b.
			* tuningItemQuality 	- The quality of the item to show in the Tuning UI. Nil if crafting. Deprecated in 1.23b.
			* tuningItemQuantity	- The amount of the item to show in the Tuning UI. Nil if crafting. Deprecated in 1.23b.
			* hqChance				- The current chance of an HQ craft.

closeCraftProgressWidget()
	Desc: Closes the crafting minigame widget.
	Params: None
	
cfmQst()
	Desc: Quest confirmation window for when starting a crafting quest from the journal.
	Params:

startRepair(craftMode, item, quality, durability, hasMateria, spiritbind)
	Desc: Opens the repair item widget.
	Params: * craftMode				- Either 0 or 1. Anything else crashes.
			* item					- ItemID of the item to be repaired.
			* quality				- Quality of the item to be repaired.
			* durability			- Durability of the item to be repaired.
			* hasMateria			- Shows an icon if the item to be repaired has materia attached.
			* spiritbind			- Spiritbind of the item to be repaired.

askJoinMateria()
displayRate()

askJoinResult(isSuccess, item, itemQuality, materia, materiaNumber, isSpiritBound)
	Desc: Opens the result widget after materia melding is done.
	Params: * isSuccess				- True if the meld was successful.
			* item					- Item ID of the melded item.
			* quality				- Quality of the melded item.
			* materia				- Item ID of the materia being melded.
			* materiaNumber			- Total count of materia on the item.
			* isSpiritBound			- True if the item is spiritbound. Causes icon to appear.
	
Notes:

Class ID + Starting skill
 29 CRP = 22550
 30 BSM = 22556
 31 ARM = 22562
 32 GSM = 22568
 33 LTW = 22574
 34 WVR = 22580
 35 ALC = 22586
 36 CUL = 22592


--]]

require ("global")


skillAnim = {
    [22553] = 0x10002000;
    [22554] = 0x10001000;
    [22555] = 0x10003000;
    [29531] = 0x10009002;
}


materialSlots = {0,0,0,0,0,0,0,0};   -- The 8 slots
recentRecipe = {10008205, 4030706, 4070009} -- Recent Recipe list
awardedRecipe = {7020105, 7030011}          -- Awarded Recipe list

materialRecipe = { -- Always 8 params because we can't have any nils here for "start" command
    [6071007] = {4070402, 4070309,0,0,0,0,0,0},
    [10008205] = {10008005,10008005,0,0,0,0,0,0},
    [10009617] = {4040009, 4040010, 4040011,0,0,0,0,0},
    [4070009] = {4070006, 10005401, 10008203,0,0,0,0,0},
    [4070010] = {10008204,10008106,10005302,0,0,0,0,0}
}

materialQuest = { -- What a quest or leve will preload slots with, in addition to any extras the player does manual
    [0] = {0,0,0,0,0,0,0,0},
    [1] = {0,0,0,0,0,0,0,0},
    [110442] = {11000075, 11000074, 0, 0, 0, 0, 0, 0}
}


function onEventStarted(player, commandactor, triggerName, arg1, arg2, arg3, arg4, checkedActorId)

    MENU_CANCEL, MENU_MAINHAND, MENU_OFFHAND, MENU_REQUEST = 0, 1, 2, 3;
    MENU_RECIPE, MENU_AWARDED, MENU_RECIPE_DETAILED, MENU_AWARDED_DETAILED = 7, 8, 9, 10;
    
    debugMessage = false;

    isRecipeRecentSent = false;
    isRecipeAwardSent = false;
    detailWindow = true;
    isRequested = false;  -- False = The default state.  True = User picked a quest recipe/local leve
    facilityId = 0;
    chosenQuest = 0;  -- Use this to store any chosen recipe/local leve
    recipeDetail = 0;
    detailWindowState = 0;
    
	craftJudge = GetStaticActor("CraftJudge");
    callClientFunction(player, "delegateCommand", craftJudge, "loadTextData", commandactor);
    
    chosenOperation = -1;
   

    while chosenOperation ~= 0 do
        
        player:ChangeState(30);
        
        if debugMessage then player:SendMessage(0x20, "", "[DEBUG] Menu ID: "..tostring(chosenOperation).."   Recipe : "..tostring(recipeMode).."   Quest : "..chosenQuest); end
        
        
        if materialQuest[chosenQuest] then
            if debugMessage then player:SendMessage(0x20, "", "Key is valid: "..chosenQuest); end
            materialSlots = materialQuest[chosenQuest];
        else
            if debugMessage then player:SendMessage(0x20, "", "Key is not valid: "..chosenQuest); end
        end

        
        if isRecipeRecentSent == false then  -- If Recipe button not hit, aka default state.
            chosenOperation, recipeMode = callClientFunction(player, "delegateCommand", craftJudge, "start", commandactor, facilityId, isRequested, unpack(materialSlots));  -- Initial window

        elseif isRecipeRecentSent == true and recipeMode == 0 then -- If recipe window/award tab was hit
            chosenOperation, recipeMode = callClientFunction(player, "delegateCommand", craftJudge, "start", commandactor, -1, isRequested, unpack(materialSlots));  -- Keep window going

        elseif isRecipeRecentSent == true and recipeMode > 0 then  -- If recipe item picked
            if recipeDetail then 
                chosenOperation, recipeMode = callClientFunction(player, "delegateCommand", craftJudge, "start", commandactor, -2, isRequested, unpack(recipeDetail)); -- Item mat(s) for picked item.
            else
                chosenOperation, recipeMode = callClientFunction(player, "delegateCommand", craftJudge, "start", commandactor, -2, isRequested, 10009617,0,0,0,0,0,0,0); -- Show dummy info for unfilled item
            end
        end     
        
        
        if chosenOperation == MENU_CANCEL then 
            callClientFunction(player, "delegateCommand", craftJudge, "closeCraftStartWidget", commandactor);
            
            
        elseif (chosenOperation == MENU_MAINHAND or chosenOperation == MENU_OFFHAND) then 
            
            if isRequested == true then
                recipeResult = callClientFunction(player, "delegateCommand", craftJudge, "selectRcp", commandactor, 10009617);
            else
                recipeResult = callClientFunction(player, "delegateCommand", craftJudge, "selectRcp", commandactor, 10009617,6071007,5030112,5030007,10009617,6071007,5030112,5030007);  
            end
            
            if recipeResult == 0 then -- Closed/Return hit.
                callClientFunction(player, "delegateCommand", craftJudge, "closeCraftStartWidget", commandactor);
                currentlyCrafting = -1;

            elseif (recipeResult >= 1 or recipeResult <= 8) then
                                                                                                --item  yld, xstal1, qty,   xstal2, qty
                recipeConfirmed = callClientFunction(player, "delegateCommand", craftJudge, "confirmRcp", commandactor, 10009617, 1, 0xF4247, 1, 0xf4245, 1, 0, 0); 

                if recipeConfirmed then
                    callClientFunction(player, "delegateCommand", craftJudge, "closeCraftStartWidget", commandactor);
                    isRecipeRecentSent = false;
                    isRecipeAwardSent = false;
                    currentlyCrafting = startCrafting(player, chosenOperation, isRequested, 80, 100, 50); 
                end
            end

        elseif chosenOperation == MENU_REQUEST then -- Conditional button label based on isRequested                 
            if isRequested == false then    -- "Request Items" hit, close Start and open up the Quest select
                callClientFunction(player, "delegateCommand", craftJudge, "closeCraftStartWidget", commandactor);
                isRecipeRecentSent = false;
                isRecipeAwardSent = false;                    
                
                local questConfirmed, returnedQuest = GetCraftQuest(player, craftjudge, commandactor);
                chosenQuest = tonumber(returnedQuest);
                
                if debugMessage then player:SendMessage(0x20, "", "[DEBUG] Chosen Quest: "..tostring(chosenQuest)); end
                    
                if questConfirmed then
                    isRequested = true;   
                end
            
                
            elseif isRequested == true then -- "Normal Synthesis" button hit   
                isRequested = false;
                chosenQuest = 0;
                callClientFunction(player, "delegateCommand", craftJudge, "closeCraftStartWidget", commandactor);
            
            end
        
        elseif chosenOperation == MENU_RECIPE then -- "Recipes" button hit
            if isRecipeRecentSent == false then
                callClientFunction(player, "delegateCommand", craftJudge, "selectRcp", commandactor, unpack(recentRecipe)); -- Load up recipe list
                isRecipeRecentSent = true;
            end

            recipeDetail = materialRecipe[recentRecipe[recipeMode]];

        elseif chosenOperation == MENU_AWARDED then -- "Awarded Recipes" tab hit  
            if isRecipeAwardSent == false then
                callClientFunction(player, "delegateCommand", craftJudge, "selectRcp", commandactor, unpack(awardedRecipe)); -- Load up Award list
                isRecipeAwardSent = true;
            end
            
            recipeDetail = materialRecipe[awardedRecipe[recipeMode]];
          
        elseif (chosenOperation == MENU_RECIPE_DETAILED or chosenOperation == MENU_AWARDED_DETAILED) and recipeMode > 0 then -- Pop-up for an item's stats/craft mats
            detailWindowState = callClientFunction(player, "delegateCommand", craftJudge, "confirmRcp", commandactor, 10009617, 1, 0xF4247, 1, 0xf4245, 1, 0, 0);

        else
            break;
        end
    end
    
    player:ChangeMusic(7); -- Need way to reset music back to the zone's default
    player:ChangeState(0);
    player:EndEvent();
	
end



-- Handles the menus to pick a crafter quest or local leve quest that run separate widgets from the Start command.
-- Returns whether a quest was selected, and what id the quest is.
function GetCraftQuest(player, craftjudge, commandactor);

    local questOffset = 0xA0F00000;
    local questId = 0;
    local requestState = false;                  
    local requestedMenuChoice = callClientFunction(player, "delegateCommand", craftJudge, "selectCraftQuest", commandactor);
    
    if requestedMenuChoice then
        questId = requestedMenuChoice - questOffset;
        
        if isCraftQuest(questId) then                                                            
            confirm = callClientFunction(player, "delegateCommand", craftJudge, "cfmQst", commandactor, questId, 20, 1, 1, 1, 0, 0, "<Path Companion>");
            
            if confirm == true then
                requestState = true;
                player:SendGameMessage(craftJudge, 21, 0x20);
            end                            
            
        elseif isLocalLeve(questId) then
            confirm = callClientFunction(player, "delegateCommand", craftJudge, "confirmLeve", commandactor, questId, 0, 8030421, 5, 50, 0, 0);
            
            if confirm == true then
                requestState = true;
                itemSlots = { unpack(materialRecipe[4070010])};
            end

        elseif isScenarioQuest(questId) == true then
            -- TEMP for now. Cannot find source for what happens if you confirm a non-craft quest.
           player:SendGameMessage(GetWorldMaster(), 40209, 0x20); 
        end
    end

    return requestState, questId;
end


function isScenarioQuest(id)

    if (id >= 110001 and id <= 120026) then
        return true;
    else
        return false;
    end
end


function isCraftQuest(id)
    if (id >= 110300 and id <= 110505) then
        return true;
    else
        return false;
    end
end


function isLocalLeve(id)

    if (id >= 120001 and id <= 120452) then
        return true;
    else
        return false;
    end
end


-- No real logic in this function.  Just smoke and mirrors to 'see' the minigame in action at the minimum level.
function startCrafting(player, hand, quest, startDur, startQly, startHQ)
    
    local worldMaster = GetWorldMaster();
    local craftProg = 0;
    local attempts = 5;
    local craftedCount = 0;
    local craftTotal = 2;
    local itemId = 10009617;
    
    player:ChangeState(30+hand);  -- Craft kneeling w/ appropriate tool out
    player:ChangeMusic(73);
    callClientFunction(player, "delegateCommand", craftJudge, "openCraftProgressWidget", commandactor, startDur, startQly, startHQ); 

    while true do 
    
        local progDiff = math.random(25,25);
        local duraDiff = math.random(1,3);
        local qltyDiff = math.random(0,2);

        if craftProg >= 100 then
        
            testChoice2 = callClientFunction(player, "delegateCommand", craftJudge, "updateInfo", commandactor,  100, 10, 20, 5020111, 69, 70, 75);
            
            -- From Lodestone: If the HQ odds are 1% or better, players will have the option of selecting either Finish or Double Down. 
            -- By electing to double down, the player takes a chance on creating an HQ item at the risk of losing the completed item if the attempt fails
            testChoice = callClientFunction(player, "delegateCommand", craftJudge, "craftTuningUI", commandactor, 22503, 22504);
            
            player:SendGameMessage(GetWorldMaster(), 40111, 0x20, player, itemId, 3, 8);  -- "You create <#3 quantity> <#1 item> <#2 quality>."
            callClientFunction(player, "delegateCommand", craftJudge, "closeCraftProgressWidget", commandactor);
            
            if quest then
                continueLeve = callClientFunction(player, "delegateCommand", craftJudge, "askContinueLocalLeve", 120001, itemId, craftedCount, craftTotal, attempts);

                if continueLeve == true then
                    craftProg = 0;
                    callClientFunction(player, "delegateCommand", craftJudge, "openCraftProgressWidget", commandactor, startDur, startQly, startHQ);
                else
                    break;
                end
            else
                break;
            end
        end
        
        choice = callClientFunction(player, "delegateCommand", craftJudge, "craftCommandUI", commandactor, 29, 2, 29530,29531,29532,29533,29534);
        --player:SendMessage(0x20, "", "[DEBUG] Command id selected: "..choice);
        

        
        if choice then
            
            if skillAnim[choice] then
                player:PlayAnimation(skillAnim[choice]);
            end
            
            wait(3);

            player:SendGameMessage(worldMaster, 40108, 0x20, choice,2);
            
            if choice ~= 29531 then
                craftProg = craftProg + progDiff;
                
                if craftProg >= 100 then 
                    craftProg = 100;
                end
                
                startDur = startDur - duraDiff;
                startQly = startQly + qltyDiff;
           
                player:SendGameMessage(worldMaster, 40102, 0x20, progDiff);
                player:SendGameMessage(worldMaster, 40103, 0x20, duraDiff);
                player:SendGameMessage(worldMaster, 40104, 0x20, qltyDiff);
            end
                                                                                                          --prg  dur  qly, ???, ???, ???,   HQ
            callClientFunction(player, "delegateCommand", craftJudge, "updateInfo", commandactor, craftProg, startDur, startQly, nil, nil, nil, nil, nil);
            
        end
    end

    return -1; 
end


