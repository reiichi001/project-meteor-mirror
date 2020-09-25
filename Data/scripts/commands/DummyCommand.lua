--[[

HarvestJudge 

Operates the harvesting system for mining, logging, and fishing.

Functions:

loadTextData(commandActor): Loads all gamesheets needed and instantiates a HarvestJudge.
targetCancel(commandActor): Cancels the player's target.
turnToTarget(commandActor, harvestType, direction): Turns to a direction (name's a lie, angle must be computed lol)
openInputWidget(commandActor, harvestType, nodeGrade): Inits the widget system (call first).
orderInputWidget(commandActor, nodeHP [max 100], ?, harvestType): Updates the node HP.
textInputWidget(commandActor, harvestType, ?, textId, ?, ?, ?): Sets the result text after a minigame is performed.
askInputWidget(commandActor, harvestType, inputPageNumber, showTutorial, showFishWait, showFishWaitAndJig, updateFishHP, showRareCatalystEffect): Gets user input after opening a ask widget.
closeInputWidget(commandActor, harvestType): Closes the widget system (call last).
rangeInputWidget(harvestType, inputPageNumber, goodMin, goodMax, bool): Unknown, currently crashing...

Notes:

HarvestType Ids:

20002: Mine
20003: Log
20004: Fish

--]]

require ("global")

function onEventStarted(player, commandactor, triggerName, arg1, arg2, arg3, arg4, checkedActorId)

	harvestJudge = GetStaticActor("HarvestJudge");
	callClientFunction(player, "delegateCommand", harvestJudge, "loadTextData", commandactor);
	callClientFunction(player, "delegateCommand", harvestJudge, "targetCancel", commandactor);
	callClientFunction(player, "delegateCommand", harvestJudge, "turnToTarget", commandactor, 0x55F2, 2);
	
	player:ChangeState(50);
	
	callClientFunction(player, "delegateCommand", harvestJudge, "openInputWidget", commandactor, 0x55F2, 2);
	callClientFunction(player, "delegateCommand", harvestJudge, "orderInputWidget", commandactor, 3, false, 0x55f2);
	callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandactor, 0x55f2, harvestJudge, nil, nil, nil, nil, 0);
	callClientFunction(player, "delegateCommand", harvestJudge, "askInputWidget", commandactor, 0x55f2, 1, 0, false, false, nil, false);
	
	callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandactor, 0x55f2, harvestJudge, 60, nil, nil, nil, 0);
	callClientFunction(player, "delegateCommand", harvestJudge, "askInputWidget", commandactor, 0x55f2, 2, 0, false, false, nil, false);
	
	callClientFunction(player, "delegateCommand", harvestJudge, "textInputWidget", commandactor, 0x55f2, harvestJudge, 46,0, 0, 0, 0);
	callClientFunction(player, "delegateCommand", harvestJudge, "askInputWidget", commandactor, 0x55f2, 2, 0, false, false, nil, false);
	
	
	player:ChangeState(0);
	
	player:EndEvent();
	
end
