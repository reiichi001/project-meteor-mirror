

function onZoneInit(zone)
end

function onZoneIn(player)

	openingQuest = player:GetQuest(110001);
	
	--Opening Quest
	if (openingQuest ~= nil) then
		if (openingQuest:GetQuestFlag(0) == false) then
			player:KickEvent(player:GetDirector(), "noticeEvent");
		end
	end	
	
end

function onZoneOut(zone, player)
end