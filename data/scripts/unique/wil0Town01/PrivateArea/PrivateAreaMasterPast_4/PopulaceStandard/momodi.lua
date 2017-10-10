require ("global")

function onSpawn(player, npc)	
	npc:SetQuestGraphic(player, 0x2);	
end

function onEventStarted(player, npc, triggerName)
	local man0u1Quest = player:GetQuest("Man0u1");
	local pos = player:GetPos();
	
	if (man0u1Quest ~= nil) then	
		callClientFunction(player, "delegateEvent", player, man0u1Quest, "processEvent010");
		player:EndEvent();
		
		--[[director = player:GetZone():CreateDirector("AfterQuestWarpDirector");
		player:KickEvent(director, "noticeEvent", true);
		player:AddDirector(director);
		player:SetLoginDirector(director);
		--]]
		GetWorldManager():DoZoneChange(player, 175, nil, 0, 15, pos[0], pos[1], pos[2], pos[3]);
		return;
	end
	
	player:EndEvent();
	
end