

function onZoneInit(zone)
end

function onZoneIn(player)

	openingQuest = player:getQuest(110001);
	
	--Opening Quest
	if (openingQuest ~= nil) then
		if (openingQuest:GetQuestFlag(0) == false) then
			player:kickEvent(player:getDirector(), "noticeEvent");
		end
	end	
	
end

function onZoneOut(zone, player)
end