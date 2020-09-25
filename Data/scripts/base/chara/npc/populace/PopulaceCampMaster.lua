--[[

PopulaceCampMaster Script

Functions:

defTalk(player, favAetheryte1, favAetheryte2, favAetheryte3, playerLevel, ?) - The main and only function for this npc. If favAetheryte1 == 0, will not ask to remove others.

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	favLocation = callClientFunction(player, "defTalk", player, 0, 1280004, 1280005);
	
	--if (hasThree) then
		--Remove chosen
	--end
	
	player:EndEvent();
end