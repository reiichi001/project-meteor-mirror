require ("global")

function init(npc)
	return false, false, 0, 0;	
end

function onEventStarted(player, npc, triggerName)
	
	harvestJudge = GetStaticActor("HarvestJudge");

	--callClientFunction(player, "delegateEvent", player, harvestJudge, "loadTextData", dummy);
	--callClientFunction(player, "delegateEvent", player, harvestJudge, "targetCancel", dummy);
	--callClientFunction(player, "delegateEvent", player, harvestJudge, "turnToTarget", dummy, 0);
	--callClientFunction(player, "delegateEvent", player, harvestJudge, "openInputWidget",  22002, 1);
	--callClientFunction(player, "delegateEvent", player, harvestJudge, "textInputWidget", harvestJudge, 22002, npc, false);
	--callClientFunction(player, "delegateEvent", player, harvestJudge, "askInputWidget", harvestJudge, 22002, 1);
	
	player:EndEvent();	
end