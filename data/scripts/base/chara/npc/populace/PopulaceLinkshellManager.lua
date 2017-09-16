--[[

PopulaceLinkshellManager Script

Functions:

eventTalkStep1(noLinkshellActive) - Says intro. If noLinkshellActive = true, say newbie stuff.
eventTalkStep2(noLinkshellActive) - Shows menu, if noLinkshellActive = true, only give ability to make linkshell.
eventTalkStepMakeupDone() - Confirm when creating LS
eventTalkStepModifyDone() - Confirm when modding LS
eventTalkStepBreakDone() - Confirm when deleting LS

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;
end

function createLinkshell(name, crest)
	
end

function modifyLinkshell(name, crest)
	
end

function disbandLinkshell(name, crest)
	
end

function onEventStarted(player, npc, triggerName)

	hasNoActiveLS = false;
	
	callClientFunction(player, "eventTalkStep1", hasNoActiveLS);
	command, lsName, crestId = callClientFunction(player, "eventTalkStep2", hasNoActiveLS);
	
	--Create
	if (result == 3) then
	
		player:SendMessage(0x20, "", "" .. tostring(lsName));
		player:SendMessage(0x20, "", "" .. tostring(crestId));
		player:SendMessage(0x20, "", "" .. tostring(command));
	
		createLinkshell(lsName, crestId);
		callClientFunction(player, "eventTalkStepMakeupDone");		
	--Modify
	elseif (result == 4) then
		modifyLinkshell(lsName, crestId);
		callClientFunction(player, "eventTalkStepModifyDone");		
	--Disband
	elseif (result == 5) then
		disbandLinkshell(lsName, crestId);
		callClientFunction(player, "eventTalkStepBreakDone");	
	end
	
	player:endEvent();
		
end