--[[

PopulaceRetainerManager Script

Functions:

eventTalkStep1(true) - Intro tutorial if no retainer
newEventTalkStep1(sayIntro) - Seems to be a post-Tanaka version of the intro????
eventTalkStep2() - Choose retainer yourself (go to race select) or let npc do it
eventTaklSelectCutSeane(cutsceneName, actorClassId1, actorClassId2, actorClassId3, actorClassId4, actorClassId5) - Starts the advance cutscene to choose a retainer. 5 retainer actorClassId's are given.
eventTalkStep4(actorClassId) - Opens up the retainer naming dialog
eventTalkStepFinalAnswer(actorClassId) - Confirm Dialog
eventTalkStepError(errorCode) - Error dialog, 1: No Extra Retainers, 2: Server Busy.
eventTalkStepFinish()


--]]

require ("global")


function init(npc)
    return false, false, 0, 0;  
end

function onEventStarted(player, npc, triggerName)
    
    local npcActorClass = npc:GetActorClassId()
    local retainerIndex = 3001100;
    local cutscene = "rtn0l010" -- Defaulting to Limsa for now for testing
    
    if npcActorClass == 1000166 then
        cutscene = "rtn0l010";
        retainerIndex = 3001101;
    elseif npcActorClass == 1000865 then
        cutscene = "rtn0u010";
        retainerIndex = 3002101;        
    elseif npcActorClass == 1001184 then
        cutscene = "rtn0g010";
        retainerIndex = 3003101;
    else
        return;
    end
  

    introChoice = callClientFunction(player, "newEventTalkStep1", false);
    
    if (introChoice == 1) then          
        
        -- Choose Retainer or Random
        raceChoice = callClientFunction(player, "eventTalkStep2");  
        
        while (true) do 

            
            if (retainerChoice == 0) then           
                raceChoice = callClientFunction(player, "eventTalkStep22"); 
            end
                
            
            if (raceChoice == 0) then 
                --Choose random actorId from a valid set for the city
                
                math.randomseed(os.time());
                local randomRetainer = math.random(retainerIndex, (retainerIndex+74));
                
                retainerName = callClientFunction(player, "eventTalkStep4", randomRetainer);

                if (retainerName ~= "") then
                    confirmChoice = callClientFunction(player, "eventTalkStepFinalAnswer", randomRetainer);

                    if (confirmChoice == 1) then
                        callClientFunction(player, "eventTalkStepFinish");
                        player:EndEvent();
                        return;
                    elseif (confirmChoice == 3) then
                        raceChoice = 0;
                    else
                        player:EndEvent();
                        return;
                    end
                else
                    callClientFunction(player, "eventTalkStepBreak");
                    raceChoice = -1;
                end 


            elseif (raceChoice > 0) and (raceChoice < 16) then                        
                --Choose 5 random but correct actor ids for the city and race/tribe
                
                local retainerRace = ((retainerIndex) + (5*(raceChoice-1)));
                local retainerRaceChoices = {retainerRace, retainerRace+1, retainerRace+2, retainerRace+3, retainerRace+4};
                
                -- Randomize the appearance order of the available five
                shuffle(retainerRaceChoices);
                
                retainerChoice = callClientFunction(player, "eventTaklSelectCutSeane", cutscene, retainerRaceChoices[1], retainerRaceChoices[2], retainerRaceChoices[3], retainerRaceChoices[4], retainerRaceChoices[5]);
                
                if (retainerChoice == -1) then
                    player:EndEvent();
                    return;             
                elseif (retainerChoice > 0) then                
                    --Retainer chosen, choose name              
                    retainerName = callClientFunction(player, "eventTalkStep4", retainerRaceChoices[retainerChoice]);
                    
                    if (retainerName ~= "") then
                        confirmChoice = callClientFunction(player, "eventTalkStepFinalAnswer", retainerRaceChoices[retainerChoice]);
                        
                        if (confirmChoice == 1) then
                            callClientFunction(player, "eventTalkStepFinish");
                            player:EndEvent();
                            return;
                        elseif (confirmChoice == 3) then
                            retainerChoice = 0;
                        else
                            player:EndEvent();
                            return;
                        end
                    else
                        callClientFunction(player, "eventTalkStepBreak");
                        raceChoice = -1;       
                    end
                    
                end 
            else
                break;
            end 
        
                
        end
        
    end
    
    player:EndEvent();
end



function shuffle(tbl)
  for i = #tbl, 2, -1 do
    local j = math.random(i)
    tbl[i], tbl[j] = tbl[j], tbl[i]
  end
  return tbl
end