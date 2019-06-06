--[[

PopulaceCompanyOfficer Script

xtx_gcRank for GC Rank values

Functions:

eventTalkWelcome()                              - Welcome dialog
eventTalkWelcomeQuest()                         - Same as Welcome dialog?
eventTalkPreJoin()                              - Dialog for starting GC rank?
eventTalkExclusive()                            - Dialog to play when you're not of that GC?
eventTalkJoinedOnly()                           - Reads like chat-end dialog for your GC.
eventTalkJoined(gcRank, gcRank,  isCanAfford, isShowPromotion)  - Menu to ask about/for promotion

eventDoRankUp(gcRank, gcRank)                   - Plays rank-up animation and opens GC window. 
eventRankUpDone(???, ???)                       - Has your character do the GC salute?  Values seem to do nothing?
eventRankCategoryUpBefore(gcRank)               - 11/21/31  - Mentions which GC quest you need to clear to continue promotion
eventRankCategoryUpAfter()                      - Follow-up dialog after ranking up
eventTalkQuestUncomplete()                      - Quest prerequisite dialog for ranking up to Second Lieutenant (1.23b rank cap)
eventTalkFestival()                             - Foundation Day 2011 event dialog.  Server needs to reward 1000 GC seals after.
eventTalkFestival2()                            - Foundation Day 2011 event dialog.  Seems to reward more seals, unsure how many.
eventTalkFestival2012(value)                    - Foundation Day 2012 event dialog.  Rewards amount of seals dictated by value, retail used 5000.

eventTalkStepBreak()                            - Resets NPC target/facing
--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

gcOfficer = { 
[1500199] = 1, -- Limsa Officer
[1500200] = 2, -- Grid Officer
[1500198] = 3, -- Flame Officer
}

function onEventStarted(player, npc, triggerName)

    playerGC = player.gcCurrent;
    playerGCSeal = 1000200 + playerGC;
    playerCurrentRank = 13;
    playerRankUpCost = 1500;
    playerNextRank = 15;
    currentRankCap = 31; -- Second Lieutenant
    npcId = npc:GetActorClassId();
    
    if playerGC == gcOfficer[npcId] then
        callClientFunction(player, "eventTalkWelcome");
        if playerCurrentRank < currentRankCap then
            if player:GetItemPackage(INVENTORY_CURRENCY):HasItem(playerGCSeal, playerRankUpCost) then
                -- Show Promotion window, allow paying
                local choice = callClientFunction(player, "eventTalkJoined", playerCurrentRank, playerNextRank, true, true);
                
                -- If promotion accepted
                if choice == 1 then
                    callClientFunction(player, "eventDoRankUp", playerNextRank, playerNextRank);
                   -- TODO: Table GC info or get it in source/sql.  Handle actual upgrade of GC rank/seal cap/cost/etc.
                end
   
            else
                -- Show Promotion window, show dialog you can't afford promotion
                callClientFunction(player, "eventTalkJoined", playerCurrentRank, playerNextRank, false, true);
            end
        else
            callClientFunction(player, "eventTalkJoined", playerCurrentRank, playerNextRank);
        end
        
        callClientFunction(player, "eventTalkJoinedOnly");
    else
        callClientFunction(player, "eventTalkExclusive");
    end
    callClientFunction(player, "eventTalkStepBreak");
    player:endEvent();
end