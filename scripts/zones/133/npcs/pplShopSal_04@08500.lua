--[[

PopulaceGuildlevePublisher Script

Functions:

welcomeTalk(say1, player, say2, say3)

openShopBuy(player, shopId) 
selectShopBuy(player)
closeShopBuy(player)

openShopSell(player, shopId
selectShopSell(player)
closeShopSell(player)

informSellPrice(num, num, num)

finishTalkTurn(nil)

selectMode(param): If >0, show class tutorial for id [param]. If <0, show gear affinity/condition instead, unless -7,-8,-9 then show normal.
selectModeOfClassVendor()
selectModeOfMultiWeaponVendor(param? is -1)
selectModeOfMultiArmorVendor(param? is -1)
confirmSellingItem()
informSellPrice(100, 100)
selectFacility
confirmUseFacility


Menu Ids:

--]]

function init(npc)
	return "/Chara/Npc/Populace/Shop/PopulaceShopSalesman", false, false, false, false, false, npc.getActorClassId(), false, false, 0, 1, "TEST";	
end

function onEventStarted(player, npc)
	player:runEventFunction("selectModeOfMultiWeaponVendor", -1); 
end

function onEventUpdate(player, npc, step, menuOptionSelected, lsName, lsCrest)

	player:runEventFunction("informSellPrice", 0x1, 0x1E, 0x46); 

	--player:runEventFunction("openShopSell", player, 0x1389); 
	--player:runEventFunction("selectShopSell", player); 
	--player:runEventFunction("closeShopSell", player); 
	--player:endEvent();		
	
end
