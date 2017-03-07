--[[

ActivateCommand Script

Switches between active and passive mode states

--]]

function onEventStarted(player, command, triggerName)	
	
	if (player:GetState() == 0) then
		player:ChangeState(2);
	elseif (player:GetState() == 2) then
		player:ChangeState(0); 
	end
		
	--player:endEvent();
	
	--For Opening Tutorial
	if (player:HasQuest("Man0l0") or player:HasQuest("Man0g0") or player:HasQuest("Man0u0")) then
		player:GetDirector("Quest/QuestDirectorMan0l001"):OnCommandEvent(player, command);	
	end	
	
end