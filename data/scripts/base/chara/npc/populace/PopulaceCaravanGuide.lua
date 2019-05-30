--[[

PopulaceCaravanGuide Script

This script handles the caravan guide class, which is for the actor who escorts the chocobos behind them during Caravan Security events.  


Functions:

caravanGuardCancel()                                        - Menu prompt to abandon the caravan

caravanGuardReward(cargo, nil, areaName, playerGC, killCount, areaName2)      
                                                            - Reward dialog for completing the caravan
                                                            - cargo = 0 (none) through 9 (all) for varying degrees of success dialog
                                                            - If playerGC doesn't match the GC of the areaName region, NPC mentions you don't need their seals.
                                                            - killCount shows an extra dialog if 40-49 enemies were slain, and a different one at 50+
                                                            
caravanGuardNotReward()                                     - Dialog stating you didn't contribute to the event at all
caravanGuardFailReward(areaName, areaName2)                 - Failure dialog, NPC offers free gysahl green, then offers free teleport back to aetheryte
caravanGuardThanks(name1, name2, name3)                     - Dialog for joining the caravan.  NPC names the three chocobos. Name IDs from xtx_displayName
caravanGuardOffer(areaName, areaName2, playerGC)            - Dialog for who to talk to for joining the caravan.
caravanGuardAmple(areaName, areaName2)                      - Dialog for NPC taking a break?
caravanGuardSuccess()                                       - Dialog when you reached destination?
caravanGuardFailure(areaName, areaName2)                    - Failure dialog for mentioned area.
caravanGuardIgnore()                                        - Resets NPC state for player?  Or used for players not flagged for the event.
caravanGuardBonusReward(nil, isBonus?)                      - NPC says variation on a piece of dialog from the boolean passed
caravanGuardNotBonusReward()                                - Inventory full flavour dialog


Notes:
Functions employing areaName/areaName2 add their value together in the client's script to get the area name.  Said area values are... 
1 = Wineport, 2 = Quarrymill, 3 = Silver Bazaar, 4 = Aleport, 5 = Hyrstmill, 6 = Golden Bazaar

areaName will always be 1-3 for caravanGuardReward to function as expected for GC-related dialog
areaName2 will always be either 0 or 3.  0 for the lower level caravan area name, 3 for the higher level.

populaceCaravanGuide sheet:
ID  Dialog                                                                                Comment
6	It is time. Come, let us ride.                                                      - Caravan begins.
12	We've arrived at last! Come and speak to me when you're ready to claim your reward. - Caravan completed.
23	We're under attack! The chocobos! Protect the chocobos!                             - Caravan aggros monsters
27	Gods, have we already come this far? At this pace, we stand to make good time.      - Says between 50% & 90% of the way to desgination? Can be said more than once per run
28	Well fought, friend. I thank the gods you're with us. Come, onward!                 - Cleared monsters that caravan aggro'd

TO-DO:
Document actors involved.  Should be six of them.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	local areaName = 1;
    local areaName2 = 3;
    local playerGC = 1;
    local cargo = 9;
    local killCount = 50;
    callClientFunction(player, "caravanGuardOffer", areaName, areaName2, playerGC);
    --callClientFunction(player, "caravanGuardReward", cargo, nil, areaName, playerGC, killCount, areaName2);   
    --player:SendGameMessageDisplayIDSender(npc, 6, MESSAGE_TYPE_SAY, npc.displayNameId);
    
    
	player:EndEvent();
end