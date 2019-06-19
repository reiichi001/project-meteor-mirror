--[[

Tutorial constants and functions

--]]

--Controller Types
CONTROLLER_KEYBOARD = 1;
CONTROLLER_GAMEPAD = 2;

--Tutorial Widget Ids

TUTORIAL_CAMERA = 1;
TUTORIAL_MOVING = 2;
TUTORIAL_TARGETING_FRIENDLY = 3;
TUTORIAL_TALKING = 4;
TUTORIAL_MAINMENU = 5;
TUTORIAL_ACTIVEMODE = 6;
TUTORIAL_TARGETING_ENEMY = 7;
TUTORIAL_ENGAGECOMBAT = 8;
TUTORIAL_BATTLEACTIONS = 9;
TUTORIAL_CASTING = 10;
TUTORIAL_ACTIONS = 11;
TUTORIAL_TP = 12;
TUTORIAL_WEAPONSKILLS = 13;
TUTORIAL_NPCS = 14;
TUTORIAL_NPCLS  = 15;
TUTORIAL_JOURNAL = 16;
TUTORIAL_DEFEATENEMY = 18;

--Helper functions

function showTutorialSuccessWidget(player, textId)
	player:SendDataPacket(2, nil, nil, textId);
end

function openTutorialWidget(player, controllerType, widgetId)
	--Default to keyboard if somethings fucky
	if (controllerType ~= CONTROLLER_GAMEPAD) then
		controllerType = CONTROLLER_KEYBOARD;
	end
	player:SendDataPacket(4, nil, nil, controllerType, widgetId);
end

function closeTutorialWidget(player)
	player:SendDataPacket(5);
end

function startTutorialMode(player)
	player:SendDataPacket(9);
end

function endTutorialMode(player)
	player:SendDataPacket(7);
end