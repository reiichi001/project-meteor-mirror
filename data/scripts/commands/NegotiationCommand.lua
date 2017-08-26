--[[

NegotiationCommand Script

Functions:

openListWidget(player, originId, 10 bools to show/hide) - Shows a list of things to Parley for based on a sequential list from xtx_negotiationTable.
openAskWidget(player, title, difficulty, desiredItemId, requiredItemId) - Opens the widget asking if the player will Parley.
openNegotiationWidget(player, title, itemId, maxTurns, time, ability1, ability2, ability3, ability4, ability5) - Inits the widget system (call first).
inputNegotiationWidget(player, ?, abort, resumeTimer) - Begins player input.
closeNegotiationWidget(player) - Closes the Parley widget.
negotiationEmote(player, animId) - Plays an animation

updateNegotiationWidget(player, gridIndex, key, itemIconId, pointValue, ?, ?) - Updates the Parley UI depending on the first argument:

< 12: Places Item
13: Plays SFX + ???
14: Sets the Negotiation Gauge (max, value)
15: Sets the Achievement Gauge (max, value)

16: Additional Item 1 (bool)
17: Additional Item 2(bool)
18: Additional Item 3(bool)

19: Set the last chosen items (index[1-6], iconId)

20:
21:

22: Clear Timer
23: Play player move SFX
24: Play opponent move SFX
25: Play times up SFX and close ability widget
26:
27:
28: Pauses the Timer
29: Resumes the Timer

--]]

require ("global")

function onEventStarted(player, commandactor, triggerName, arg1, arg2, arg3, arg4, arg5)
	
	negotiationJudge = GetStaticActor("NegotiationJudge");

	callClientFunction(player, "delegateCommand", negotiationJudge, "negotiationEmote", player, 403087360);
	--callClientFunction(player, "delegateCommand", negotiationJudge, "openAskWidget", player, 1302, 5, 1000019, 1000019);
	--callClientFunction(player, "delegateCommand", negotiationJudge, "openListWidget", player, 3711, true, true, true, true, false, false, false, false, false, false);
	callClientFunction(player, "delegateCommand", negotiationJudge, "openNegotiationWidget", player, 1302, 1000019, 15, 20, 0, 0, 0, 0, 0);
	callClientFunction(player, "delegateCommand", negotiationJudge, "updateNegotiationWidget", player, 2, 2007, 60662, 5, false, false);
	callClientFunction(player, "delegateCommand", negotiationJudge, "inputNegotiationWidget", player, 1, false, true);
	callClientFunction(player, "delegateCommand", negotiationJudge, "closeNegotiationWidget", player);
	
	player:EndEvent();	
end