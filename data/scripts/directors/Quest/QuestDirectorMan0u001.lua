require ("global")
require ("quests/man/man0u0")

--processTtrBtl001: Active Mode Tutorial
--processTtrBtl002: Targetting Tutorial (After active mode done)
--processTtrBtl003: Auto Attack Done
--processTtrBtl004: Tutorial Complete

--[[
12: TP
13: WeaponSkills

]]--

function init()
	return "/Director/Quest/QuestDirectorMan0u001";
end

function onEventStarted(player, actor, triggerName)	

	quest = GetStaticActor("Man0u0");
	callClientFunction(player, "delegateEvent", player, quest, "processTtrBtl001", nil, nil, nil);
	callClientFunction(player, "delegateEvent", player, quest, "processTtrBtl002", nil, nil, nil);
	
	--sendDataPacket: Success
	--sendDataPacket: CloseWidget
	--IF DoW:
		--sendDataPacket: OpenWidget (TP)
		--IF TP REACHED:
		--sendDataPacket: CloseWidget
		--sendDataPacket: OpenWidget (WS)
		--IF WS USED:
		--sendDataPacket: Success
		--sendDataPacket: CloseWidget
	--ELSE MAGIC:
		--sendDataPacket: OpenWidget (DEFEAT ENEMY)
			
	--IF DEAD
	--sendDataPacket: Attention
	
	player:EndEvent();
	
end

function onUpdate()
end

function onTalkEvent(player, npc)

end

function onPushEvent(player, npc)
end

function onCommandEvent(player, command)

	quest = GetStaticActor("Man0l0");
	callClientFunction(player, "delegateEvent", player, quest, "processTtrBtl002", nil, nil, nil);	

end

function onEventUpdate(player, npc)
end

function onCommand(player, command)	
end