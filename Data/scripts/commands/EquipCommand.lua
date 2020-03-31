--[[

EquipCommand Script

Notes: 

Gearset activating could be optimized a bit more by doing the item packets in one go.

The param "equippedItem" has the vars: actorId, unknown, slot, and inventoryType. 
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

function onEventStarted(player, actor, triggerName, equippedItem, param1, param2, param3, param4, param5, param6, param7, equipSlot, itemDBIds)
	equipSlot = equipSlot-1;
	
	--Equip Item
	if (equippedItem ~= nil) then		
		item = player:GetItemPackage(equippedItem.itemPackage):GetItemAtSlot(equippedItem.slot);		
		equipItem(player, equipSlot, item);			
		player:SendAppearance();
	--Unequip Item
	else	
		item = player:GetEquipment():GetItemAtSlot(equipSlot);
		if (unequipItem(player, equipSlot, item) == true) then --Returns true only if something changed (didn't error out)
			player:SendAppearance();
		end
	end
	
	player.CalculateBaseStats(); --player.RecalculateStats();
	
	player:EndEvent();	
end

function loadGearset(player, classId)	
	player:GetEquipment():ToggleDBWrite(false);
	local gearset = player:GetGearset(classId);
	
	if gearset == nil then
		return;
	end
	
	for slot = 0, 34 do
	
		if (slot ~= EQUIPSLOT_MAINHAND and slot ~= EQUIPSLOT_UNDERSHIRT and slot ~= EQUIPSLOT_UNDERGARMENT) then
			itemAtSlot = player:GetEquipment():GetItemAtSlot(slot);
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
	player:GetEquipment():ToggleDBWrite(true);	
end

function equipItem(player, equipSlot, item)
	if (item ~= nil) then	
		local classId = nil;
		local worldMaster = GetWorldMaster();
		local gItem = GetItemGamedata(item.itemId);
		
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
				player:SendGameMessage(player, worldMaster, 30103, 0x20, 0, 0, player, classId); 
				player:PrepareClassChange(classId);
			end
				
		end		
		
		--Item Equipped message
		player:SendGameMessage(player, worldMaster, 30601, 0x20, equipSlot+1, item.itemId, item.quality, 0, 0, 1); 
		
		--Load gearset for new class and begin class change
		if (classId ~= nil) then			
			loadGearset(player, classId);
			player:DoClassChange(classId);
		end

		player:GetEquipment():Set(equipSlot, item);		
		
		--EquipSlot -> GraphicSlot
		if 	   (equipSlot == EQUIPSLOT_MAINHAND and gItem:IsNailWeapon() == false) then graphicSlot = GRAPHICSLOT_MAINHAND;
		elseif (equipSlot == EQUIPSLOT_OFFHAND) then graphicSlot = GRAPHICSLOT_OFFHAND;
		elseif (equipSlot == EQUIPSLOT_THROWINGWEAPON) then graphicSlot = GRAPHICSLOT_THROWING;
		elseif (equipSlot == EQUIPSLOT_PACK) then graphicSlot = GRAPHICSLOT_PACK;
		elseif (equipSlot == EQUIPSLOT_HEAD) then graphicSlot = GRAPHICSLOT_HEAD;
		elseif (equipSlot == EQUIPSLOT_BODY) then graphicSlot = GRAPHICSLOT_BODY;
		elseif (equipSlot == EQUIPSLOT_LEGS) then graphicSlot = GRAPHICSLOT_LEGS;
		elseif (equipSlot == EQUIPSLOT_HANDS) then graphicSlot = GRAPHICSLOT_HANDS;
		elseif (equipSlot == EQUIPSLOT_FEET) then graphicSlot = GRAPHICSLOT_FEET;
		elseif (equipSlot == EQUIPSLOT_WAIST) then graphicSlot = GRAPHICSLOT_WAIST;
		elseif (equipSlot == EQUIPSLOT_NECK) then graphicSlot = GRAPHICSLOT_NECK;	
		elseif (equipSlot == EQUIPSLOT_RFINGER) then graphicSlot = GRAPHICSLOT_R_RINGFINGER;
		elseif (equipSlot == EQUIPSLOT_LFINGER) then graphicSlot = GRAPHICSLOT_L_RINGFINGER;
		end
				
		--Special cases for WVR and GSM. Offhand goes to the special offhand slot.
		if (equipSlot == EQUIPSLOT_OFFHAND) then
			if (gItem:IsWeaverWeapon() == true) then graphicSlot = GRAPHICSLOT_SPOFFHAND; end
			if (gItem:IsGoldSmithWeapon() == true) then graphicSlot = GRAPHICSLOT_SPOFFHAND; end
		end
				
		--Graphic Slot was set, otherwise it's a special case
		if (graphicSlot ~= nil) then
			player:GraphicChange(graphicSlot, item);
		elseif (gItem:IsNailWeapon()) then
			player:GraphicChange(GRAPHICSLOT_MAINHAND, item);
		elseif (equipSlot == EQUIPSLOT_EARS) then
			player:GraphicChange(GRAPHICSLOT_R_EAR, item);
			player:GraphicChange(GRAPHICSLOT_L_EAR, item);
		elseif (equipSlot == EQUIPSLOT_WRIST) then
			player:GraphicChange(GRAPHICSLOT_R_WRIST, item);
			player:GraphicChange(GRAPHICSLOT_L_WRIST, item);
		end		
		
		--Special graphics for crafting classes
		if (classId ~= nil) then					
			if (gItem:IsCarpenterWeapon()) then
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 898,4,0,0);
				player:GraphicChange(GRAPHICSLOT_SPOFFHAND,  898,4,0,0);
			elseif (gItem:IsBlackSmithWeapon()) then
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 899,1,0,0);
				player:GraphicChange(GRAPHICSLOT_SPOFFHAND,  899,1,0,0);
			elseif (gItem:IsArmorerWeapon()) then
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 899,2,0,0);
				player:GraphicChange(GRAPHICSLOT_SPOFFHAND,  899,2,0,0);
			elseif (gItem:IsGoldSmithWeapon()) then
				player:GraphicChange(GRAPHICSLOT_OFFHAND, 	 729,1,0,0);
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 898,1,0,0);
			elseif (gItem:IsTannerWeapon()) then
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 898,3,0,0);
				player:GraphicChange(GRAPHICSLOT_SPOFFHAND,  898,3,0,0);
			elseif (gItem:IsWeaverWeapon()) then 
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 000,0,0,0);
			elseif (gItem:IsAlchemistWeapon()) then 
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 900,1,0,0);
			elseif (gItem:IsCulinarianWeapon()) then 
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 900,2,0,0);
				player:GraphicChange(GRAPHICSLOT_SPOFFHAND,  898,2,0,0);
			else
				player:GraphicChange(GRAPHICSLOT_SPMAINHAND, 0,0,0,0);
				player:GraphicChange(GRAPHICSLOT_SPOFFHAND,  0,0,0,0);
			end
		end
		
	end
end

function unequipItem(player, equipSlot, item)
	worldMaster = GetWorldMaster();

	if (item ~= nil and (equipSlot == EQUIPSLOT_MAINHAND or equipSlot == EQUIPSLOT_UNDERSHIRT or equipSlot == EQUIPSLOT_UNDERGARMENT)) then
		player:SendGameMessage(player, worldMaster, 30730, 0x20, equipSlot+1, item.itemId, item.quality, 0, 0, 1); --Unable to unequip
	elseif (item ~= nil) then
		player:SendGameMessage(player, worldMaster, 30602, 0x20, equipSlot+1, item.itemId, item.quality, 0, 0, 1); --Item Removed
		player:GetEquipment():Clear(equipSlot);
				
		if (equipSlot == EQUIPSLOT_BODY) then --Show Undershirt
			item = player:GetEquipment():GetItemAtSlot(EQUIPSLOT_UNDERSHIRT);
			player:GraphicChange(GRAPHICSLOT_BODY, item);
		elseif (equipSlot == EQUIPSLOT_LEGS) then --Show Undergarment
			item = player:GetEquipment():GetItemAtSlot(EQUIPSLOT_UNDERGARMENT);
			player:GraphicChange(GRAPHICSLOT_LEGS, item);			
		elseif  (equipSlot == EQUIPSLOT_HANDS) then player:GraphicChange(15, 0, 1, 0, 0);
		elseif  (equipSlot == EQUIPSLOT_FEET) then player:GraphicChange(16, 0, 1, 0, 0);
		else
			if 	   (equipSlot == EQUIPSLOT_MAINHAND) then player:GraphicChange(GRAPHICSLOT_MAINHAND, nil);
			elseif (equipSlot == EQUIPSLOT_OFFHAND) then player:GraphicChange(GRAPHICSLOT_OFFHAND, nil);
			elseif (equipSlot == EQUIPSLOT_THROWINGWEAPON) then player:GraphicChange(GRAPHICSLOT_THROWING, nil);
			elseif (equipSlot == EQUIPSLOT_PACK) then player:GraphicChange(GRAPHICSLOT_PACK, nil);
			elseif (equipSlot == EQUIPSLOT_HEAD) then player:GraphicChange(GRAPHICSLOT_HEAD, nil);
			elseif (equipSlot == EQUIPSLOT_WAIST) then player:GraphicChange(GRAPHICSLOT_WAIST, nil);
			elseif (equipSlot == EQUIPSLOT_NECK) then  player:GraphicChange(GRAPHICSLOT_NECK, nil);
			elseif (equipSlot == EQUIPSLOT_EARS) then player:GraphicChange(GRAPHICSLOT_L_EAR, nil); player:GraphicChange(GRAPHICSLOT_R_EAR, nil);
			elseif (equipSlot == EQUIPSLOT_WRIST) then player:GraphicChange(GRAPHICSLOT_L_WRIST, nil); player:GraphicChange(GRAPHICSLOT_R_WRIST, nil);
			elseif (equipSlot == EQUIPSLOT_RFINGER) then player:GraphicChange(GRAPHICSLOT_R_RINGFINGER, nil);
			elseif (equipSlot == EQUIPSLOT_LFINGER) then player:GraphicChange(GRAPHICSLOT_L_RINGFINGER, nil);
			end
		end	
		
		return true;
	end
end
