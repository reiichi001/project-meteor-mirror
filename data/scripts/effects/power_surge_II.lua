require("modifiers")
require("hiteffect")
require("battleutils")

--https://www.bluegartr.com/threads/107403-Stats-and-how-they-work/page22
function onGain(owner, effect)
    owner.AddMod(modifiersGlobal.Attack, 230);
    owner.SubtractMod(modifiersGlobal.Defense, 158);
end

function onCommandStart(effect, owner, command, actionContainer)
    --if command is a weaponskill or jump
    --27266: jump
    if command.GetCommandType() == CommandType.Weaponskill or command.id == 27266 then
        effect.SetTier(effect.GetTier() + 1);
        
        --Takes 10 weaponskills/jumps to increase level
        if effect.GetTier() > 10 then
            local action = owner.statusEffects.ReplaceEffect(effect, 223214, 1, 1, 60);
            actionContainer.AddAction(action);
        else
            effect.RefreshTime();
        end
    end
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Attack, 230);
    owner.AddMod(modifiersGlobal.Defense, 158);
end

