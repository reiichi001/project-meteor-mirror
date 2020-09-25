--[[

AetheryteChild Script

Functions:

eventAetheryteChildSelect(showTeleport, parentAetheryteID, animaAmount, animaCost(always 1)): Opens menu
eventAetheryteChildDesion(aetheryteId): "Your homepoint is now X"

eventGLSelect(?) - Open GL selector
eventGLSelectDetail(glid, ?, reward, rewardQuantity, subreward, subrewardQuantity, faction, ?, completed) - Show GL details
eventGLDifficulty() - Open difficulty selector
eventGLStart(glId, difficulty, evaluatingFaction, areaFactionStanding, factionReward, warningBoundByDuty, warningTooFar, warningYouCannotRecieve, warningChangingClass) - Confirmation dialog

eventGLBoost(currentFavor, minNeeded) - Ask player for Guardian Aspect
eventGLPlay(??) - Open Menu (GL active version)
eventGLReward (glId, clearTime, missionBonus, difficultyBonus, factionNumber, factionBonus, factionCredit, reward, rewardQuantity, subreward, subrewardQuantity, difficulty) - Open reward window
eventGLJoin () - Ask to join party leader's leve

--]]

require ("global")
require ("aetheryte")
require ("utils")
require ("guildleve")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, aetheryte, triggerName)	
	
	if (player:GetGuildleveDirector() ~= nil) then
		doGuildleveMenu(player, aetheryte);
	else
		doNormalMenu(player, aetheryte);
	end
	
	player:EndEvent();
end

function doGuildleveMenu(player, aetheryte)

	local currentGLDirector = player:GetGuildleveDirector();	
	local choice = callClientFunction(player, "eventGLPlay", currentGLDirector.guildleveId, true, 1, 500, 400, guardian, 8, currentGLDirector.selectedDifficulty, 2);

	--Abandon
	if (choice == 6) then
		currentGLDirector:AbandonGuildleve();
	end

end

function doNormalMenu(player, aetheryte)

	local aetheryteId = aetheryte:GetActorClassId();
	local parentNode = aetheryteChildLinks[aetheryteId];
	local menuChoice = callClientFunction(player, "eventAetheryteChildSelect", true, parentNode, 100, 1);
	
	--Teleport
	if (menuChoice == 2) then
		printf("%ud", parentNode);
		destination = aetheryteTeleportPositions[parentNode];
		
		if (destination ~= nil) then
			randoPos = getRandomPointInBand(destination[2], destination[4], 3, 5);
			rotation = getAngleFacing(randoPos.x, randoPos.y, destination[2], destination[4]);
			GetWorldManager():DoZoneChange(player, destination[1], nil, 0, 2, randoPos.x, destination[3], randoPos.y, rotation);
		end
	--Init Levequest
	elseif (menuChoice == -1) then
		doLevequestInit(player, aetheryte);
	--Set Homepoint
	elseif (menuChoice == -2) then
		player:SetHomePoint(aetheryteId);
		callClientFunction(player, "eventAetheryteChildDesion", aetheryteId);
	--View Faction Standing
	elseif (menuChoice == -3) then
		player:SendGameMessage(player, aetheryte, 27, 0x20);
		player:SendGameMessage(player, aetheryte, 28, 0x20, 1, 15);
		player:SendGameMessage(player, aetheryte, 29, 0x20, 2, 10);
		player:SendGameMessage(player, aetheryte, 30, 0x20, 3, 5);
	end
end

function doLevequestInit(player, aetheryte)
	local worldMaster = GetWorldMaster();
	::SELECT_LOOP::
	unknown, glId = callClientFunction(player, "eventGLSelect", 0x0);
	if (glId ~= 0) then
		::SELECT_DETAIL::
		guildleveData = GetGuildleveGamedata(glId);
		
		if (guildleveData == nil) then
			player:SendMessage(0x20, "", "An error has occured... aborting.");
			return;
		end
		
		unknown, begin = callClientFunction(player, "eventGLSelectDetail", glId, 0xa, 0xf4241, 1000, 0, 0, 0, true, false);		
		if (begin) then
			::SELECT_DIFFICULTY::
			player:SendGameMessage(worldMaster, 50014, 0x20); --"Please select a difficulty level. This may be lowered later."
			difficulty = callClientFunction(player, "eventGLDifficulty", glId);			
			if (difficulty == nil) then goto SELECT_DETAIL; end			
			confirmResult = callClientFunction(player, "eventGLStart", glId, difficulty, 1, 10, 20, 0, 0, 0, 0);
			if (confirmResult == nil) then goto SELECT_DIFFICULTY; else
			
				player:SendGameMessage(worldMaster, 50036, 0x20, glId, player);
				player:PlayAnimation(getGLStartAnimationFromSheet(guildleveData.borderId, guildleveData.plateId, true));				
				director = player:GetZone():CreateGuildleveDirector(glId, difficulty, player);
				player:AddDirector(director);
				director:StartDirector(true, glId)
			
			end
		else
			goto SELECT_LOOP;
		end
	end
end
