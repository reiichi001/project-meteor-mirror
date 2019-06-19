--[[

PopulaceCaravanAdviser Script

Functions:

adviserDeffault()       - Not a typo.  NPC dialog talking about a chocobo.  Resets their sight on you, perhaps used on closing dialog?
adviserAsk()            - Brings up a menu for caravan info, or purchasing gysahl greens
adviserAdvise()         - NPC dialog discussing feeding chocobos
adviserSales(price)     - Gysahl purchase dialog and prompt
adviserBuy()            - Dialog to play after purchasing gysahl greens
adviserBuyNG()          - NPC plays /shrug animation.  

--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	local gysahlPrice = 20;
	local choice = callClientFunction(player, "adviserAsk");
    
    if choice == 1 then
        callClientFunction(player, "adviserAdvise");
    elseif choice == 2 then
        local purchaseChoice = callClientFunction(player, "adviserSales", gysahlPrice);
        
        if purchaseChoice == 1 then
            callClientFunction(player, "adviserBuy");
        elseif purchaseChoice == 2 then
            callClientFunction(player, "adviserBuyNG");
        end
    elseif choice == 3 then
        callClientFunction(player, "adviserDeffault")
    end

	player:EndEvent();
end