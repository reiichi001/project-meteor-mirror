--[[

Common Retainer Stuff

Retainer Say Codes:

1: Hired
2: When called
3: Error when cannot call retainer
4: Dismissed
5: ??
6: Sold X items report.
7: Nothing got sold.
8: Retainer payed???
9: Retainer dismissed due to not paid.
10: Retainer dismissed by player.


--]]

function doItemTrade(player, retainer)
	callClientFunction(player, "eventTalkRetainerItemTrade", 1);
	
	while (true) do
		resultCode, type7Param, un1, quantity, itemId, unk2 = callClientFunction(player, "eventTalkRetainerItemTrade", 2);
		
		--Retreieve
		if (resultCode == 31) then
			--UpdatePlayer
			--UpdateRetainer
		--Entrust
		elseif (resultCode == 32) then
			--UpdatePlayer
			--UpdateRetainer
		end
		
		callClientFunction(player, "eventReturnResult", resultCode, false);
		
		if (resultCode == 100) then
			break
		end
	end
	
	callClientFunction(player, "eventTalkRetainerItemTrade", 3);
end

function doBazaar(player, retainer)
	callClientFunction(player, "eventTalkRetainerItemList", 1);
	callClientFunction(player, "eventTalkRetainerItemList", 2);	
	callClientFunction(player, "eventTalkRetainerItemList", 3);
end