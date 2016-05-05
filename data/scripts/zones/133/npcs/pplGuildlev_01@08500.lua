--[[

PopulaceGuildlevePublisher Script

Functions:

eventTalkType(level (changes factionLeves), sayIntro, brokenBladePoints, shieldsPoints, hornhandPoints, showTutorialLeves, doOmen (!=0), menuId (to Jump), leveAllowances, ?, ?, ?)
eventTalkPack(startGuildlevePack, endGuildlevePack)
eventTalkCard(card1,card2,card3,card4,card5,card6,card7,card8)
eventTalkDetail(guildLeveId, factionEvaluating, rewardType1, rewardQuantity1, rewardType2, rewardQuantity2, boostPoint, previouslyCompleted, completionBonus)
eventTalkAfterOffer()
eventHistoryleveExist(guildLeveId)
eventHistoryleveCannot()
eventGLChangeDetail(?, guildLeveId, boostPoint, rewardType1, rewardQuantity1, rewardType2, rewardQuantity2, factionEvaluating, previouslyCompleted)
eventTalkChangeOne(skipQuestion)
talkOfferMaxOver()
askRetryRegionalleve(guildLeveId, leveAllowances);

Menu Ids:

--]]

function init(npc)
	return "/Chara/Npc/Populace/PopulaceGuildlevePublisher", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	player:runEventFunction("eventTalkType", 0x30, true, 0x02CE, 0x356, 0x367, true, 0, nil, 0x29, 0,0,0);
end

function onEventUpdate(player, npc, step, menuOptionSelected)
	--player:runEventFunction("eventTalkType", 0x32, true, 0x02CE, 0x356, 0x367, false, 2, nil, 0x29, 0,0,0);
	player:runEventFunction("eventTalkPack", 201, 207);
	--player:runEventFunction("eventTalkCard", 0x30C3, 0x30C4, 0x30C1, 0x30C5, 0x30C6, 0x30C7, 0x30C8, 0x30C9);
	--player:runEventFunction("eventTalkDetail", 0x30C4, 2, 0xF4242, 0xD, 0xF4242, 0, 0xFF, true, 11);
	--player:runEventFunction("eventGLChangeDetail", 0xDEAD, 0x30C4, 0xFF, 0xF4242, 0xD, 0xF4242, 0, 2, true);
	player:endEvent();	
end