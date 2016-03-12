local initClassItems, initRaceItems;

function onLogin(player)
	player:sendMessage(0x1D,"",">Callback \"onLogin\" for player script running.");
	
	if (player:getPlayTime(false) == 0) then
		player:sendMessage(0x1D,"",">PlayTime == 0, new player!");
		
		initClassItems(player);
		initRaceItems(player);
	end
	
end

function initClassItems(player)

	local slotTable;
	local invSlotTable;

	--DoW	
	if (player.charaWork.parameterSave.state_mainSkill[0] == 2) then 		--PUG
		player:getInventory(0):addItem({4020001, 8030701, 8050728, 8080601, 8090307});
		player:getEquipment():SetEquipment({0, 10, 12, 14, 15},{0, 1, 2, 3, 4});
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 3) then	--GLA
		player:getInventory(0):addItem({4030010, 8031120, 8050245, 8080601, 8090307});
		player:getEquipment():SetEquipment({0, 10, 12, 14, 15},{0, 1, 2, 3, 4});
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 4) then	--MRD
		player:getInventory(0):addItem({4040001, 8011001, 8050621, 8070346, 8090307});
		player:getEquipment():SetEquipment({0, 8, 12, 13, 15},{0, 1, 2, 3, 4});
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 7) then	--ARC
		player:getInventory(0):addItem({4070001, 8030601, 8050622, 8080601, 8090307});
		player:getEquipment():SetEquipment({0, 10, 12, 14, 15},{0, 1, 2, 3, 4});
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 8) then	--LNC
		player:getInventory(0):addItem({4080201, 8030801, 8051015, 8080501, 8090307});
		player:getEquipment():SetEquipment({0, 10, 12, 14, 15},{0, 1, 2, 3, 4});
	--DoM	
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 22) then	--THM
		player:getInventory(0):addItem({5020001, 8030245, 8050346, 8080346, 8090208});
		player:getEquipment():SetEquipment({0, 10, 12, 14, 15},{0, 1, 2, 3, 4});
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 23) then	--CNJ
		player:getInventory(0):addItem({5030101, 8030445, 8050031, 8080246, 8090208});
		player:getEquipment():SetEquipment({0, 10, 12, 14, 15},{0, 1, 2, 3, 4});
		
	--DoH
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 29) then	--
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 30) then	--
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 31) then	--
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 32) then	--
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 33) then	--
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 34) then	--
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 35) then	--
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 36) then	--
	
	--DoL
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 39) then	--MIN
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 40) then	--BTN	
	elseif (player.charaWork.parameterSave.state_mainSkill[0] == 41) then	--FSH
	end
	
end

function initRaceItems(player)

	if (player.playerWork.tribe == 1) then		--Hyur Midlander Male
		player:getInventory(0):addItem(8040001);
		player:getInventory(0):addItem(8060001);
	elseif (player.playerWork.tribe == 2) then	--Hyur Midlander Female
		player:getInventory(0):addItem(8040002);
		player:getInventory(0):addItem(8060002);
	elseif (player.playerWork.tribe == 3) then	--Hyur Highlander Male
		player:getInventory(0):addItem(8040003);
		player:getInventory(0):addItem(8060003);
	elseif (player.playerWork.tribe == 4) then	--Elezen Wildwood Male
		player:getInventory(0):addItem(8040004);
		player:getInventory(0):addItem(8060004);
	elseif (player.playerWork.tribe == 5) then	--Elezen Wildwood Female
		player:getInventory(0):addItem(8040006);
		player:getInventory(0):addItem(8060006);
	elseif (player.playerWork.tribe == 6) then	--Elezen Duskwight Male
		player:getInventory(0):addItem(8040005);
		player:getInventory(0):addItem(8060005);
	elseif (player.playerWork.tribe == 7) then	--Elezen Duskwight Female
		player:getInventory(0):addItem(8040007);
		player:getInventory(0):addItem(8060007);
	elseif (player.playerWork.tribe == 8) then	--Lalafell Plainsfolk Male
		player:getInventory(0):addItem(8040008);
		player:getInventory(0):addItem(8060008);
	elseif (player.playerWork.tribe == 9) then	--Lalafell Plainsfolk Female
		player:getInventory(0):addItem(8040010);
		player:getInventory(0):addItem(8060010);
	elseif (player.playerWork.tribe == 10) then	--Lalafell Dunesfolk Male
		player:getInventory(0):addItem(8040009);
		player:getInventory(0):addItem(8060009);
	elseif (player.playerWork.tribe == 11) then	--Lalafell Dunesfolk Female
		player:getInventory(0):addItem(8040011);
		player:getInventory(0):addItem(8060011);
	elseif (player.playerWork.tribe == 12) then	--Miqo'te Seekers of the Sun
		player:getInventory(0):addItem(8040012);
		player:getInventory(0):addItem(8060012);
	elseif (player.playerWork.tribe == 13) then	--Miqo'te Seekers of the Moon
		player:getInventory(0):addItem(8040013);
		player:getInventory(0):addItem(8060013);
	elseif (player.playerWork.tribe == 14) then	--Roegadyn Sea Wolf
		player:getInventory(0):addItem(8040014);
		player:getInventory(0):addItem(8060014);
	elseif (player.playerWork.tribe == 15) then	--Roegadyn Hellsguard
		player:getInventory(0):addItem(8040015);
		player:getInventory(0):addItem(8060015);
	end

	player:getEquipment():SetEquipment({9, 11},{5,6});
	
end