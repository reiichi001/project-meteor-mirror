--[[

PopulacePassiveGLPublisher Script

Functions:

askOfferPack() - Show Classes
askOfferRank() - Show Ranks
askOfferQuest(player)
confirmOffer(nil, questId)
confirmMaxOffer()
talkOfferWelcome(actor, leveAllowances)
talkOfferDecide()
talkOfferMaxOver()
selectDiscardGuildleve(player)
confirmJournal()
askDiscardGuildleve()
confirmDiscardGuildleve(nil, questId)
askRetryRegionalleve(questId, leveAllowances)
finishTalkTurn()

Menu Ids:

--]]

function init(npc)
	return "/Chara/Npc/Populace/PopulacePassiveGLPublisher", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	player:runEventFunction("talkOfferWelcome", player, 1);
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)
	--player:runEventFunction("askOfferQuest", player, 1000);
	player:endEvent();
end