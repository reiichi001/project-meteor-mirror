require("global");
require("modifiers");
properties = {
    permissions = 0,
    parameters = "s",
    description = 
[[
equips all your class and job actions
]],
}

classToActions = {
    [2] = { Start = 27100, End = 27119},
    [3] = { Start = 27140, End = 27159},
    [4] = { Start = 27180, End = 27199},
    [7] = { Start = 27220, End = 27239},
    [8] = { Start = 27260, End = 27279},
    [22] = { Start = 27300, End = 27319},
    [23] = { Start = 27340, End = 27359}
}

function onTrigger(player, argc)
    local messageId = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "equipactions";

    classId = player.GetClass()

    if classToActions[classId] then
        s = classToActions[classId].Start
        e = classToActions[classId].End
        print('h')
        for i = 0, 30 do
            player.UnequipAbility(i, false)
        end
        
        for commandid = s, e do
            if GetWorldManager():GetBattleCommand(commandid) then
                player:EquipAbilityInFirstOpenSlot(player:GetCurrentClassOrJob(), commandid);
            end
        end
    end
end