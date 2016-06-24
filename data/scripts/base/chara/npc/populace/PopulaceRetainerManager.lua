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
	
	introChoice = callClientFunction(player, "newEventTalkStep1", false);
	
	if (introChoice == 1) then			
					
		raceChoice = callClientFunction(player, "eventTalkStep2");
		
		while (true) do	
		
			if (retainerChoice == 0) then			
				raceChoice = callClientFunction(player, "eventTalkStep22");	
			end
			
			if (raceChoice == 0) then
				--Choose random actorId
			elseif (raceChoice > 0) then						
				--Choose 5 random but correct actor ids			
				retainerChoice = callClientFunction(player, "eventTaklSelectCutSeane", "rtn0g010", 0x2DCB1A, 0x2DCB1A, 0x2DCB1A, 0x2DCB1A, 0x2DCB1A);
				
				if (retainerChoice == -1) then
					player:EndEvent();
					return;				
				elseif (retainerChoice > 0) then				
					--Retainer chosen, choose name				
					retainerName = callClientFunction(player, "eventTalkStep4", 0x2DCB1A);
					
					if (retainerName ~= "") then
						confirmChoice = callClientFunction(player, "eventTalkStepFinalAnswer", 0x2DCB1A);
						
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
						
					end
					
				end	
			else
				break;
			end		
				
		end
		
	end
	
	player:EndEvent();
end