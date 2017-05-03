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

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, aetheryte, triggerName)

	aetheryteId = aetheryte:GetActorClassId();
	parentNode = aetheryteChildLinks[aetheryteId];
	menuChoice = callClientFunction(player, "eventAetheryteChildSelect", true, parentNode, 100, 1);
	
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
	
	player:EndEvent();
end

function doLevequestInit(player, aetheryte)
	::SELECT_LOOP::
	unknown, glId = callClientFunction(player, "eventGLSelect", 0x0);
	if (glId ~= 0) then
		::SELECT_DETAIL::
		unknown, begin = callClientFunction(player, "eventGLSelectDetail", glId, 0xa, 0xf4241, 1000, 0, 0, 0, true, false);
		if (begin) then
			::SELECT_DIFFICULTY::
			difficulty = callClientFunction(player, "eventGLDifficulty", glId);			
			if (difficulty == nil) then goto SELECT_DETAIL; end			
			confirmResult = callClientFunction(player, "eventGLStart", glId, difficulty, 1, 10, 20, 0, 0, 0, 0);
			if (confirmResult == nil) then goto SELECT_DIFFICULTY; else
			end
		else
			goto SELECT_LOOP;
		end
	end
end
