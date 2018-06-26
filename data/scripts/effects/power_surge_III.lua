require("modifiers")
require("battleutils")

--https://www.bluegartr.com/threads/107403-Stats-and-how-they-work/page22
function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.Attack, 345);
    owner.SubtractMod(modifiersGlobal.Defense, 158);
end

function onCommandStart(effect, owner, command, actionContainer)
    --if command is a weaponskill or jump
    --27266: jump
    if command.GetCommandType() == CommandType.Weaponskill or command.id == 27266 then
        --At III just refresh the effect
        effect.RefreshTime();
    end
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Attack, 345);
    owner.AddMod(modifiersGlobal.Defense, 158);
end

