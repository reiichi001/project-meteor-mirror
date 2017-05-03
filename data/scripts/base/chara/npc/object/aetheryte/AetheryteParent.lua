--[[

AetheryteParent Script

Functions:

eventAetheryteParentSelect(showTeleportMenu, animaAmount, gate1, gate2, gate3, gate4, gate5) - Open Menu
eventAetheryteParentDesion(sheetId) - Show Homepoint Set Message

eventGLSelect(?) - Open GL selector
eventGLSelectDetail(glid, ?, reward, rewardQuantity, subreward, subrewardQuantity, faction, ?, completed) - Show GL details
eventGLDifficulty() - Open difficulty selector
eventGLStart(glId, difficulty, evaluatingFaction, areaFactionStanding, factionReward, warningBoundByDuty, warningTooFar, warningYouCannotRecieve, warningChangingClass) - Confirmation dialog

eventGLBoost(currentFavor, minNeeded) - Ask player for Guardian Aspect
eventGLPlay(??) - Open Menu (GL active version)
eventGLReward (glId, clearTime, missionBonus, difficultyBonus, factionNumber, factionBonus, factionCredit, reward, rewardQuantity, subreward, subrewardQuantity, difficulty) - Open reward window
eventGLJoin () - Ask to join party leader's leve


	--callClientFunction(player, "eventGLBoost", 0xc8, 0xb);	
	--callClientFunction(player, "eventGLReward", 0x2a48, 120, 123, 125, 1, 111, 0, 0xf4241, 5, 0, 0, 3);

--]]

require ("global")
require ("aetheryte")
require ("utils")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, aetheryte, triggerName)
		
	local aetheryteId = aetheryte:GetActorClassId();
	local childNodes = aetheryteParentLinks[aetheryteId];
	
	local listPosition = 1;
	local activeChildNodes = {0, 0, 0, 0, 0};
	
	if (childNodes ~= nil) then
		if (player:HasAetheryteNodeUnlocked(childNodes[1])) then
			activeChildNodes[listPosition] = childNodes[1];
			listPosition = listPosition+1;
		end
		if (player:HasAetheryteNodeUnlocked(childNodes[2])) then
			activeChildNodes[listPosition] = childNodes[2];
			listPosition = listPosition+1;
		end
		if (player:HasAetheryteNodeUnlocked(childNodes[3])) then
			activeChildNodes[listPosition] = childNodes[3];
			listPosition = listPosition+1;
		end
		if (player:HasAetheryteNodeUnlocked(childNodes[4])) then
			activeChildNodes[listPosition] = childNodes[4];
			listPosition = listPosition+1;
		end
		if (player:HasAetheryteNodeUnlocked(childNodes[5])) then
			activeChildNodes[listPosition] = childNodes[5];
			listPosition = listPosition+1;
		end
	end
	
	local showTeleportOptions = true;
	if (listPosition == 1) then
		showTeleportOptions = false;
	end
	
	local choice = callClientFunction(player, "eventAetheryteParentSelect", showTeleportOptions, 100, activeChildNodes[1], activeChildNodes[2], activeChildNodes[3], activeChildNodes[4], activeChildNodes[5]);
	
	if (choice ~= nil) then 
		--Init Leavequest
		if (choice == -1) then
			doLevequestInit(player, aetheryte);
		--Set Homepoint
		elseif (choice == -2) then
			player:SetHomePoint(aetheryteId);
			callClientFunction(player, "eventAetheryteParentDesion", aetheryteId);
		--View Faction Standings
		elseif (choice == -3) then
			player:SendGameMessage(player, aetheryte, 124, 0x20);
			player:SendGameMessage(player, aetheryte, 125, 0x20, 1, 15);
			player:SendGameMessage(player, aetheryte, 126, 0x20, 2, 10);
			player:SendGameMessage(player, aetheryte, 127, 0x20, 3, 5);
		--Teleport to Gate
		elseif (choice > 0) then
			destination = aetheryteTeleportPositions[activeChildNodes[choice]];
			if (destination ~= nil) then
				randoPos = getRandomPointInBand(destination[2], destination[4], 3, 5);
				rotation = getAngleFacing(randoPos.x, randoPos.y, destination[2], destination[4]);
				GetWorldManager():DoZoneChange(player, destination[1], nil, 0, 2, randoPos.x, destination[3], randoPos.y, rotation);
			end
		end		
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
