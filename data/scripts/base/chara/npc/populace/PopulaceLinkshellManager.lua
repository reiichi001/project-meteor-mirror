--[[

PopulaceLinkshellManager Script

Functions:

eventTalkStep1(noLinkshellActive) - Says intro. If noLinkshellActive = true, say newbie stuff.
eventTalkStep2(noLinkshellActive) - Shows menu, if noLinkshellActive = true, only give ability to make linkshell.
eventTalkStepMakeupDone() - Confirm when creating LS
eventTalkStepModifyDone() - Confirm when modding LS
eventTalkStepBreakDone() - Confirm when deleting LS

Text IDs:

25121 - That [@SWITCH($E8(1),linkshell,company)] name is already being used.
25122 - That [@SWITCH($E8(1),linkshell,company)] name cannot be used.
25123 - The [@SWITCH($E8(1),linkshell,company)] “[@STRING($EA(2))]” has been [@SWITCH($E8(1),created,founded)].

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;
end

function createLinkshell(player, name, crest)
	GetWorldManager():RequestWorldLinkshellCreate(player, name, crest);
	return waitForSignal("ls_result");	
end

function modifyLinkshell(player, name, crest)
	
end

function disbandLinkshell(player, name, crest)
	
end

function onEventStarted(player, npc, triggerName)

	local hasNoActiveLS = false;
	
	callClientFunction(player, "eventTalkStep1", hasNoActiveLS);
	local command, lsName, crestId = callClientFunction(player, "eventTalkStep2", hasNoActiveLS);
	
	--Create
	if (command == 3) then	
		local result = createLinkshell(player, lsName, crestId);
		if (result == 0) then
			callClientFunction(player, "eventTalkStepMakeupDone");
		elseif (result == 1) then
			player:SendGameMessage(player, GetWorldMaster(), 25121, 0x20); --LS already exists
			callClientFunction(player, "eventTalkStepBreakDone");
		elseif (result == 2) then
			player:SendGameMessage(player, GetWorldMaster(), 25122, 0x20); --Cannot use this name (reserved/banned)
			callClientFunction(player, "eventTalkStepBreakDone");
		elseif (result == 3) then
		end
	--Modify
	elseif (command == 4) then
		modifyLinkshell(player, lsName, crestId);
		callClientFunction(player, "eventTalkStepModifyDone");		
	--Disband
	elseif (command == 5) then
		disbandLinkshell(player, lsName, crestId);
		callClientFunction(player, "eventTalkStepBreakDone");	
	end
		
	player:endEvent();
		
end