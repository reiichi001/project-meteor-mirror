--[[

PopulaceSpecialEventCryer Script

Actor Class script to handle the 6 NPCs (technically 3, the actors were duped) involved in the Foundation Day 2011 & 2012 events.  
In 2011 they appear to be used for recruitment information for their respective Grand Company.
In 2012, they were used for exchanging Over-aspected Crystals/Clusters for GC seals as part of the ongoing Atomos event.

Functions:

For 2011.
eventTalkStep0(joined)      - NPC dialog about joining their cause to fight back Imperials. joined = 0 or 1. Function has hardcoded actor IDs, won't work with 2012 versions
eventTalkNotGCmenber(npcGC) - NPC dialog when you're not part of their grand company.

For 2012.
eventTalkCrystalExchange(player, npcGC, hasCrystal) - NPC dialog explaining they want over-aspected crystals. Brings up crystal exchange prompt if hasCrystal = 1.
eventTalkCsOverflow(player, npcGC)                  - Error message that you can't hold the seals being offered.
eventTalkCrystalExchange2(player, npcGC)            - NPC dialog for accepting exchange of crystals for seals

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end


local gcRep = { 
    [1001619] = 1, -- Maelstrom Representative 2011
    [1002105] = 1, -- Maelstrom Representative 2012
    [1001623] = 2, -- Adder Representative 2011
    [1002109] = 2, -- Adder Representative 2012
    [1001627] = 3, -- Flame Representative 2011
    [1002113] = 3, -- Flame Representative 2012
}


function onEventStarted(player, npc, triggerName)
    local playerGC = player.gcCurrent;
    local npcId = npc:GetActorClassId();
    local npcGC = gcRep[npcId]; 
    local npcGCSeal = 1000200 + npcGC;
    local hasCrystal = 1;
    local crystal = 3020537;
    local cluster = 3020413;
    local eventMode = 2012;
    
    
    if eventMode == 2011 then
        if playerGC == 0 then
            callClientFunction(player, "eventTalkStep0", 0);
        elseif playerGC == npcGC then
            callClientFunction(player, "eventTalkStep0", 1); 
        else
            callClientFunction(player, "eventTalkNotGCmenber", npcGC);
        end
    
    elseif eventMode == 2012 then
        choice = callClientFunction(player, "eventTalkCrystalExchange", player, npcGC, hasCrystal);
        
        if choice == 1 then
            --callClientFunction(player, "eventTalkCsOverflow", player, npcGC);
            player:SendMessage(0x20, "", "You pretend to hand over four over-aspected crystals.");
            callClientFunction(player, "eventTalkCrystalExchange2", player, npcGC); 
           
            local invCheck = player:GetItemPackage(INVENTORY_CURRENCY):AddItem(npcGCSeal, 1000, 1);
            if invCheck == INV_ERROR_SUCCESS then
                player:SendGameMessage(player, GetWorldMaster(), 25071, MESSAGE_TYPE_SYSTEM, crystal, 1, npcGCSeal, 1, 4, 1000);
            end
        elseif choice == 2 then
            player:SendMessage(0x20, "", "You pretend to hand over an over-aspected cluster.");
            --callClientFunction(player, "eventTalkCsOverflow", player, npcGC);
            callClientFunction(player, "eventTalkCrystalExchange2", player, npcGC); 
            
            local invCheck = player:GetItemPackage(INVENTORY_CURRENCY):AddItem(npcGCSeal, 3000, 1);
            if invCheck == INV_ERROR_SUCCESS then
                player:SendGameMessage(player, GetWorldMaster(), 25071, MESSAGE_TYPE_SYSTEM, cluster, 1, npcGCSeal, 1, 1, 3000);
            end            
        end
    end
	
	player:EndEvent();
end