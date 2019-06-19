require ("global")

function onSpawn(player, npc)	
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)
	local man0g1Quest = player:GetQuest("Man0g1");
	local pos = player:GetPos();
	
	if (man0g1Quest ~= nil) then	
		callClientFunction(player, "delegateEvent", player, man0g1Quest, "processEvent110");
		player:EndEvent();
		
		--[[director = player:GetZone():CreateDirector("AfterQuestWarpDirector");
		player:KickEvent(director, "noticeEvent", true);
		player:AddDirector(director);
		player:SetLoginDirector(director);
		--]]
		GetWorldManager():DoZoneChange(player, 155, nil, 0, 15, pos[0], pos[1], pos[2], pos[3]);
		return;
	end
	
	player:EndEvent();
	
end