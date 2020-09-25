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

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)
	callClientFunction(player, "talkOfferWelcome", player, 1);
	player:EndEvent();
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)
	--callClientFunction(player, "askOfferQuest", player, 1000);
	
end