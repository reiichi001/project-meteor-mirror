--[[

PopulaceGuildShop Script

In 1.20, the devs removed Guild Marks as acquirable.  In 1.21, this class was set up to allow exchanging them for 
a variety of materia/crystals/gil, as well as refunding traits purchased with marks.  Traits used to be purchased
to slot in, where-as with late-XIV they are just automatically unlocked once the appropriate level is met.

Functions:

cashbackTalkCommand(arg1 through arg10)             -- Dialog for refunding purchased skills prior to Job update.  Args are xtx_command values for command names.
cashbackTalk(nil, refundAmount, arg3 through arg10) -- Dialog for refunding treaties to guild marks.  Arg3 through 10 use xtx_itemName values.
selectMode(nil, npcId, isShowExchange, guildCurrency, unk) -- Menus for exchanging leftover marks, undoing class points, and learning about guild.  Unk seems related to point resetting

maskShopListIndex(shopPack?, isSomething)           -- Presumably hides an item in the shop list.  Needs to be called after openShopBuy or errors client.
guildExplain(npcId, player)                         -- Guild Mark tutorial dialog.  selectMode calls this on its own

--]]

require ("global")
require ("shop")

function init(npc)
    return false, false, 0, 0;  
end

guildShopInfo = { -- [actor id] = { saySheetId, guildmarkCurrency }
[1000157] = {9,  1000103}, -- Marauder, S'raemha
[1000158] = {24, 1000120}, -- Culinarian, Noline
[1000162] = {18, 1000114}, -- Blacksmith, Qhas Chalahko
[1000164] = {16, 1000123}, -- Fishermen, Faucillien 
[1000459] = {21, 1000117}, -- Leatherworker, Gallia
[1000460] = {13, 1000111}, -- Conjurer, Hetzkin
[1000461] = {15, 1000122}, -- Botanist, Kipopo
[1000462] = {11, 1000107}, -- Lancer, Clarembald
[1000464] = {10, 1000106}, -- Archer, Cassandra
[1000466] = {17, 1000113}, -- Carpenter, Frances
[1000631] = {8,  1000102}, -- Gladiator, Coynach
[1000632] = {7,  1000101}, -- Pugilist, Moruith
[1000633] = {12, 1000110}, -- Thaumaturge, Nyunoeya
[1000634] = {23, 1000119}, -- Alchemist, Kylene
[1000635] = {20, 1000116}, -- Goldsmith, Hnaufrid
[1000636] = {22, 1000118}, -- Weaver, Lafla Morfla
[1000637] = {14, 1000121}, -- Miner, Shilgen
[1001461] = {19, 1000115}, -- Armorer, Notrelchamps
}



function onEventStarted(player, npc)

    local npcId = npc:GetActorClassId();
    local saySheetId = guildShopInfo[npcId][1];
    local shopCurrency = guildShopInfo[npcId][2];
    local gilCurrency = 1000001;
    local keepersHymn = 3020410;
    local shopPack = 0;
    
    callClientFunction(player, "welcomeTalk", nil, saySheetId, player);

    while (true) do 
        local choice = callClientFunction(player, "selectMode", nil, npcId, true, shopCurrency, 100);

        if (choice == 3) then       -- Undo Point Allotment
            -- TODO: Add point reset handling
        elseif (choice == 4) then   -- Leave menu selected
            player:EndEvent();
            break;
        elseif (choice == nil) then -- Escape key hit to leave menu
            player:EndEvent();
            break
        elseif (choice >= 102 and choice <= 120) then -- Exchange marks for Materia
            shopPack = choice + 18;     -- Index offset
            if (choice == 119) then     
                shopPack = shopPack + 1;
            elseif (choice == 120) then  -- Exchange marks for Crystals
                shopPack = 144;
            end;
            processGuildShop(player, shopPack, shopCurrency);
        elseif (choice == 121) then -- Exchange marks for Gil.  1 mark = 4 gil
            local markAmount = player:GetItemPackage(INVENTORY_CURRENCY):GetItemQuantity(shopCurrency);
            purchaseItem(player, INVENTORY_CURRENCY, gilCurrency, markAmount*4, 1, markAmount, shopCurrency);
            
        end
    end
    
    player:EndEvent()
end



function processGuildShop(player, choice, currency)

        callClientFunction(player, "openShopBuy", player, choice, currency);
        --callClientFunction(player, "maskShopListIndex", 137, true);

        while (true) do     
            buyResult, quantity = callClientFunction(player, "selectShopBuy", player);
            
            if (buyResult == 0) then
                callClientFunction(player, "closeShopBuy", player);                 
                break;
            else 
              player:SendMessage(0x20, "", string.format("Player purchased %s item(s) at index %s in shopPack %s.", quantity, buyResult, choice));
            end
        end     
end
