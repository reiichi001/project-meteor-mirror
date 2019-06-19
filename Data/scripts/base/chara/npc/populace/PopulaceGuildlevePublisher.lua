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

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc)

	::MENU_LOOP::
	menuChoice = callClientFunction(player, "eventTalkType", 0x30, true, 0x02CE, 0x356, 0x367, true, 0, nil, 0x29, 0,0,0);	
	--Battlecraft
	if (menuChoice == 1) then
		resultGLPack = callClientFunction(player, "eventTalkPack", 201, 207);
		
		if (resultGLPack == nil) then
			goto MENU_LOOP;
		else
		
			::CARDS_LOOP::
			cards = {0x30C3, 0x30C4, 0x30C1, 0x30C5, 0x30C6, 0x30C7, 0x30C8, 0x30C9};
		
			chosenGLCard = callClientFunction(player, "eventTalkCard", cards[1], cards[2], cards[3], cards[4], cards[5], cards[6], cards[7], cards[8]);
			
			if (chosenGLCard == -1) then
				goto MENU_LOOP;
			else
				wasAccepted = callClientFunction(player, "eventTalkDetail", cards[chosenGLCard], 0, 0xF4242, 0xD, 0xF4242, 0, 0, true, 0);
				
				if (wasAccepted == true) then
				
				end
				
				goto CARDS_LOOP;
				
			end
		end
		
	--FieldCraft Miner
	elseif (menuChoice == 0x15) then
	--FieldCraft Botanist
	elseif (menuChoice == 0x16) then
	--FieldCraft Fisher
	elseif (menuChoice == 0x17) then
	--FieldCraft Quit
	elseif (menuChoice == 0x18) then
	--Faction Broken Blade
	elseif (menuChoice == 0x29) then
	--Faction Shields
	elseif (menuChoice == 0x2A) then
	--Faction Horn and Hand
	elseif (menuChoice == 0x2B) then
	--Leve Evaluation
	elseif (menuChoice == 5) then
	--Tutorial
	elseif (menuChoice == 6) then
	--End of Info
	elseif (menuChoice == 7) then	
	--Quit
	elseif (menuChoice == 8) then	
	end
	
	--
	--
	--
	player:EndEvent();
end

function onEventUpdate(player, npc, step, menuOptionSelected)
	--player:RunEventFunction("eventTalkType", 0x32, true, 0x02CE, 0x356, 0x367, false, 2, nil, 0x29, 0,0,0);
	player:RunEventFunction("eventTalkPack", 201, 207);
	--player:RunEventFunction("eventTalkCard", 0x30C3, 0x30C4, 0x30C1, 0x30C5, 0x30C6, 0x30C7, 0x30C8, 0x30C9);
	--
	--player:RunEventFunction("eventGLChangeDetail", 0xDEAD, 0x30C4, 0xFF, 0xF4242, 0xD, 0xF4242, 0, 2, true);
	
end