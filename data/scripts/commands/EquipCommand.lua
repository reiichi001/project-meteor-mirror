--[[

EquipCommand Script

Notes: 

Gearset activating could be optimized a bit more by doing the item packets in one go.

The param "invActionInfo" has the vars: actorId, unknown, slot, and inventoryType. 
The param "itemDBIds" has the vars: item1 and item2. 

--]]

EQUIPSLOT_MAINHAND 			= 0;
EQUIPSLOT_OFFHAND 			= 1;
EQUIPSLOT_THROWINGWEAPON 	= 4;
EQUIPSLOT_PACK 				= 5;
EQUIPSLOT_POUCH	 			= 6;
EQUIPSLOT_HEAD 				= 8;
EQUIPSLOT_UNDERSHIRT		= 9;
EQUIPSLOT_BODY				= 10;
EQUIPSLOT_UNDERGARMENT		= 11;
EQUIPSLOT_LEGS 				= 12;
EQUIPSLOT_HANDS 			= 13;
EQUIPSLOT_FEET 				= 14;
EQUIPSLOT_WAIST 			= 15;
EQUIPSLOT_NECK 				= 16;
EQUIPSLOT_EARS 				= 17;
EQUIPSLOT_WRIST 			= 19;
EQUIPSLOT_RFINGER 			= 21;
EQUIPSLOT_LFINGER 			= 22;

GRAPHICSLOT_MAINHAND 		= 5;
GRAPHICSLOT_OFFHAND 		= 6;
GRAPHICSLOT_SPMAINHAND 		= 7;
GRAPHICSLOT_SPOFFHAND 		= 8;
GRAPHICSLOT_THROWING 		= 9;
GRAPHICSLOT_PACK			= 10;
GRAPHICSLOT_POUCH			= 11;
GRAPHICSLOT_HEAD 			= 12;
GRAPHICSLOT_BODY			= 13;
GRAPHICSLOT_LEGS 			= 14;
GRAPHICSLOT_HANDS 			= 15;
GRAPHICSLOT_FEET 			= 16;
GRAPHICSLOT_WAIST 			= 17;
GRAPHICSLOT_NECK 			= 18;
GRAPHICSLOT_R_EAR 			= 19;
GRAPHICSLOT_L_EAR 			= 20;
GRAPHICSLOT_R_WRIST			= 21;
GRAPHICSLOT_L_WRIST			= 22;
GRAPHICSLOT_R_RINGFINGER 	= 23;
GRAPHICSLOT_L_RINGFINGER	= 24;
GRAPHICSLOT_R_INDEXFINGER 	= 25;
GRAPHICSLOT_L_INDEXFINGER 	= 26;

function onEventStarted(player, actor, triggerName, invActionInfo, param1, param2, param3, param4, param5, param6, param7, equipSlot, itemDBIds)
	equipSlot = equipSlot-1;
	
	--Equip Item
	if (invActionInfo ~= nil) then		
		item = player:getInventory(0):getItemBySlot(invActionInfo.slot);		
		equipItem(player, equipSlot, item);			
		player:sendAppearance();
	--Unequip Item
	else	
		item = player:getEquipment():GetItemAtSlot(equipSlot);
		if (unequipItem(player, equipSlot, item) == true) then --Returns true only if something changed (didn't error out)
			player:sendAppearance();
		end
	end
	
	player:endCommand();	
end

function loadGearset(player, classId)	
	player:getEquipment():ToggleDBWrite(false);
	local gearset = player:getGearset(classId);
	
	if gearset == nil then
		return;
	end
	
	for slot = 0, 34 do
	
		if (slot ~= EQUIPSLOT_MAINHAND and slot ~= EQUIPSLOT_UNDERSHIRT and slot ~= EQUIPSLOT_UNDERGARMENT) then
			itemAtSlot = player:getEquipment():GetItemAtSlot(slot);
			itemAtGearsetSlot = gearset[slot];
			
			if (itemAtSlot ~= nil or itemAtGearsetSlot ~= nil) then		
				if	   (itemAtSlot ~= nil and itemAtGearsetSlot == nil) then
					unequipItem(player, slot, itemAtSlot);
				elseif (itemAtSlot == nil and itemAtGearsetSlot ~= nil) then
					equipItem(player, slot, itemAtGearsetSlot);
				elseif (itemAtGearsetSlot.uniqueId ~= itemAtSlot.uniqueId) then
					unequipItem(player, slot, itemAtSlot);
					equipItem(player, slot, itemAtGearsetSlot)
				end
			end
		end
		
	end
	
	player:getEquipment():ToggleDBWrite(true);
	
end

function equipItem(player, equipSlot, item)
	if (item ~= nil) then	
		local classId = nil;
		local worldMaster = getWorldMaster();
		local gItem = getItemGamedata(item.itemId);
		
		--If it's the mainhand, begin class change based on weapon
		if (equipSlot == EQUIPSLOT_MAINHAND) then
			if 	   (gItem:IsNailWeapon()) then classId = 2;
			elseif (gItem:IsSwordWeapon()) then classId = 3;
			elseif (gItem:IsAxeWeapon()) then classId = 4;
			elseif (gItem:IsBowWeapon()) then classId = 7;
			elseif (gItem:IsLanceWeapon()) then classId = 8;
			
			elseif (gItem:IsThaumaturgeWeapon()) then classId = 22;
			elseif (gItem:IsConjurerWeapon()) then classId = 23;
			
			elseif (gItem:IsCarpenterWeapon()) then classId = 29;
			elseif (gItem:IsBlackSmithWeapon()) then classId = 30;
			elseif (gItem:IsArmorerWeapon()) then classId = 31;
			elseif (gItem:IsGoldSmithWeapon()) then classId = 32;
			elseif (gItem:IsTannerWeapon()) then classId = 33;
			elseif (gItem:IsWeaverWeapon()) then classId = 34;
			elseif (gItem:IsAlchemistWeapon()) then classId = 35;
			elseif (gItem:IsCulinarianWeapon()) then classId = 36;
			
			elseif (gItem:IsMinerWeapon()) then classId = 39;
			elseif (gItem:IsBotanistWeapon()) then classId = 40;
			elseif (gItem:IsFishingWeapon()) then classId = 41;
			end	
			
			if (classId ~= nil) then
				player:sendGameMessage(player, worldMaster, 30103, 0x20, 0, 0, player, classId); 
				player:prepareClassChange(classId);
			end
				
		end		
		
		--Item Equipped message
		player:sendGameMessage(player, worldMaster, 30601, 0x20, equipSlot+1, item.itemId, item.quality, 0, 0, 1); 
		
		player:getEquipment():Equip(equipSlot, item);		
		
		if 	   (equipSlot == EQUIPSLOT_MAINHAND and gItem:IsNailWeapon() == false and gItem:IsBowWeapon() == false) then graphicSlot = GRAPHICSLOT_MAINHAND;
		elseif (equipSlot == EQUIPSLOT_OFFHAND) then graphicSlot = GRAPHICSLOT_OFFHAND;
		elseif (equipSlot == EQUIPSLOT_HEAD) then graphicSlot = GRAPHICSLOT_HEAD;
		elseif (equipSlot == EQUIPSLOT_BODY) then graphicSlot = GRAPHICSLOT_BODY;
		elseif (equipSlot == EQUIPSLOT_LEGS) then graphicSlot = GRAPHICSLOT_LEGS;
		elseif (equipSlot == EQUIPSLOT_HANDS) then graphicSlot = GRAPHICSLOT_HANDS;
		elseif (equipSlot == EQUIPSLOT_FEET) then graphicSlot = GRAPHICSLOT_FEET;
		elseif (equipSlot == EQUIPSLOT_WAIST) then graphicSlot = GRAPHICSLOT_WAIST;
		elseif (equipSlot == EQUIPSLOT_RFINGER) then graphicSlot = GRAPHICSLOT_RFINGER;
		elseif (equipSlot == EQUIPSLOT_LFINGER) then graphicSlot = GRAPHICSLOT_LFINGER;
		end
		
		--Graphic Slot was set, otherwise it's a special case
		if (graphicSlot ~= nil) then
			player:graphicChange(graphicSlot, item);
			if (graphicSlot == GRAPHICSLOT_MAINHAND) then player:graphicChange(GRAPHICSLOT_OFFHAND, nil); end
		elseif (gItem:IsNailWeapon()) then
			player:graphicChange(GRAPHICSLOT_MAINHAND, item);
			player:graphicChange(GRAPHICSLOT_OFFHAND, item);
		elseif (gItem:IsBowWeapon()) then
			player:graphicChange(GRAPHICSLOT_MAINHAND, item);
			--player:graphicChange(GRAPHICSLOT_OFFHAND, item);
		elseif (equipSlot == EQUIPSLOT_EARS) then
			player:graphicChange(GRAPHICSLOT_R_EAR, item);
			player:graphicChange(GRAPHICSLOT_L_EAR, item);
		end
	
		--Load gearset for new class and begin class change
		if (classId ~= nil) then			
			loadGearset(player, classId);
			player:doClassChange(classId);
		end
		
	end
end

function unequipItem(player, equipSlot, item)
	worldMaster = getWorldMaster();

	if (item ~= nil and (equipSlot == EQUIPSLOT_MAINHAND or equipSlot == EQUIPSLOT_UNDERSHIRT or equipSlot == EQUIPSLOT_UNDERGARMENT)) then
		player:sendGameMessage(player, worldMaster, 30730, 0x20, equipSlot+1, item.itemId, item.quality, 0, 0, 1); --Unable to unequip
	elseif (item ~= nil) then
		player:sendGameMessage(player, worldMaster, 30602, 0x20, equipSlot+1, item.itemId, item.quality, 0, 0, 1); --Item Removed
		player:getEquipment():Unequip(equipSlot);
				
		if (equipSlot == EQUIPSLOT_BODY) then --Show Undershirt
			item = player:getEquipment():GetItemAtSlot(EQUIPSLOT_UNDERSHIRT);
			player:graphicChange(GRAPHICSLOT_BODY, item);
		elseif (equipSlot == EQUIPSLOT_LEGS) then --Show Undergarment
			item = player:getEquipment():GetItemAtSlot(EQUIPSLOT_UNDERGARMENT);
			player:graphicChange(GRAPHICSLOT_LEGS, item);			
		elseif  (equipSlot == EQUIPSLOT_HANDS) then player:graphicChange(15, 0, 1, 0, 0);
		elseif  (equipSlot == EQUIPSLOT_FEET) then player:graphicChange(16, 0, 1, 0, 0);
		else
			if 	   (equipSlot == EQUIPSLOT_MAINHAND) then player:graphicChange(GRAPHICSLOT_MAINHAND, nil);
			elseif (equipSlot == EQUIPSLOT_OFFHAND) then player:graphicChange(GRAPHICSLOT_OFFHAND, nil);
			elseif (equipSlot == EQUIPSLOT_HEAD) then player:graphicChange(GRAPHICSLOT_HEAD, nil);
			elseif (equipSlot == EQUIPSLOT_WAIST) then player:graphicChange(GRAPHICSLOT_WAIST, nil);
			elseif (equipSlot == EQUIPSLOT_EARS) then player:graphicChange(GRAPHICSLOT_L_EAR, nil); player:graphicChange(GRAPHICSLOT_R_EAR, nil);
			elseif (equipSlot == EQUIPSLOT_RFINGER) then player:graphicChange(GRAPHICSLOT_RFINGER, nil);
			elseif (equipSlot == EQUIPSLOT_LFINGER) then player:graphicChange(GRAPHICSLOT_LFINGER, nil);
			end
		end	
		return true;
	end
end
