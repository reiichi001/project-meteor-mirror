--[[

PartyTargetCommand Script

Handles placing marks on targets

--]]
require("global")

markers = {         -- [id] = {overheadIcon, textIcon}
    [0] = {0, 0},     -- Clear  
    [1] = {1000, 304},-- Watch my HP!
    [2] = {2000, 305},-- Watch my MP!
    [3] = {3000, 306},-- Watch my TP!
    [5] = {5000, 308},-- I need enhancing magic!
    [6] = {6000, 309},-- I am enfeebled!
    [7] = {7000, 310},-- Good!
    [8] = {8000, 311},-- Bad!
    
    [100] = {-7000, 296}, -- Attack this target!
    [101] = {-6000, 297}, -- Focus on this target!
    [102] = {-5000, 298}, -- Stop this target!
    [104] = {-4000, 299}, -- Do not attack this target!
    [105] = {-3000, 300}, -- General mark Spade
    [106] = {-2000, 301}, -- General mark Club
    [107] = {-1000, 302}, -- General mark Diamond
}


function onEventStarted(player, actor, triggerName, commandValue, category, unk1, unk2, targetActor, unk3, unk4, unk5, unk6)
	
    workName = "charaWork.parameterTemp.targetInformation";
    uiFunc = "charaWork/stateForAll";
    
    markerIndex = markers[commandValue][1] or 0;
    iconIndex = markers[commandValue][2] or 0;
    categoryKind = tonumber(category) or -1;
    worldMaster = GetWorldMaster();

    if categoryKind == -1 then
        return
    end
    
    player:SetWorkValue(player, workName, uiFunc, markerIndex);
    
    if iconIndex != 0 then
        if categoryKind == 1 then
            player:SendGameMessage(player, worldMaster, 30422, 0x20, player, iconIndex);
        elseif categoryKind == 2 then
            player:SendGameMessage(player, worldMaster, 30412, 0x20, player, iconIndex);
        end
    elseif iconIndex == 0 then
        player:SendGameMessage(player, worldMaster, 30413, 0x20, player, 0);
    end

	player:EndEvent();
end