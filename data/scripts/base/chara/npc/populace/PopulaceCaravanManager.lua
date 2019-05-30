--[[

PopulaceCaravanManager Script

Functions:

caravanGuardEntry(areaGC, hasRoomForGCSeals, areaName, difficulty, playerGC, playerCountRequired, levelRequired) 
       - Dialog for signing up for caravan. areaGC(1-3) & areaName(0 or 3) added together to get location name.  
       - If difficulty => 40 on areaGC 1-3 & areaName 0, NPC mentions it's be a tougher trip
       
caravanGuardQuestion(areaName1, areaName2, escortMax, isSameGC?, playerGC?) - Ask about the caravan escort
caravanGuardJoinOK(areaName1, areaName2, playerGC)   - Dialog for successfully joining the caravan
caravanGuardJoinNG(nil, maxEscorts, playerGC)        - Dialog dictating how many escorts total filled the run.  
caravanGuardAmple(nil, playerGC, playerGC)           - Dialog for caravan escort being full.
caravanGuardOther(npcGC)                             - Dialog where NPC mentions you're not part of the given Grand Company parameter
caravanGuardSigh()                                   - NPC does a shrug animation
caravanGuardHuh()                                    - NPC does /huh
caravanGuardCancel(nil, playerGC)                    - Dialog for canceling caravan escort. 


Notes:
Some NPC dialog address you differently if your GC rank is Chief Sergeant (id 27) or higher, but only in non-English languages.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	local GC = 3;
    local playerGC = 1; 
    local areaName = 0;
    local level = 25;
    local playerCount = 8;
    local difficulty = 41;
    local hasRoomForGCSeals = false;
	local isSameGC = true;
    local escortMax = 8;
    areaName1 = 1;
    areaName2 = 3;
    
   -- callClientFunction(player, "caravanGuardCancel", nil, 3);
    
    callClientFunction(player, "caravanGuardEntry", GC, hasRoomForGCSeals, areaName, difficulty, playerGC, playerCount, level);
	player:EndEvent();
end