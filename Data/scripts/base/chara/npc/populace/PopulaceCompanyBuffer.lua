--[[

PopulaceCompanyBuffer Script

Functions:

eventTalkWelcome(player, boolean)       - Welcome dialog.  Boolean seems to be related to rank?
eventTalkBufEffect()                    - Dialog for applying Sanction
eventTalkBufEffectAfter(player)         - Dialog after applying Sanction
eventTalkStepBreak()                    - Returns to NPC's neutral state

--]]

require ("global")

function init(npc)
    return false, false, 0, 0;  
end

local gcRep = { 
    [1500388] = 1, -- Maelstrom Representative
    [1500389] = 2, -- Adder Representative
    [1500390] = 3, -- Flame Representative
}

function onEventStarted(player, npc, triggerName)
    local playerGC = player.gcCurrent;
    local playerGCRanks = {player.gcRankLimsa, player.gcRankGridania, player.gcRankUldah};
    
    local choice = callClientFunction(player, "eventTalkWelcome", player, true);
    
    if (choice == 1 and playerGCRanks[playerGC] > 10 and playerGCRanks[playerGC] < 112) then
        callClientFunction(player, "eventTalkBufEffect");
        callClientFunction(player, "eventTalkBufEffectAfter", player);
        -- TODO: Add Sanction buff
    else
        player:SendMessage(0x20, "", "Quit hex editing your memory.");
    end
   
    callClientFunction(player, "eventTalkStepBreak"); 
    player:endEvent();
end

