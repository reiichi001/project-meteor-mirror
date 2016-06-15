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
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)
	player:RunEventFunction("talkOfferWelcome", player, 1);
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)
	--player:RunEventFunction("askOfferQuest", player, 1000);
	player:EndEvent();
end