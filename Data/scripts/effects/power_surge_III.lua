require("modifiers")
require("battleutils")

--https://www.bluegartr.com/threads/107403-Stats-and-how-they-work/page22
function onGain(owner, effect, actionContainer)
    local attackGained = 315;
    local defenseLost = 158;

    --Enhanced Power Surge: Increases effect of Power Surge by 10% (assuming this doesn't lower defense further)
    if owner.HasTrait(27281) then
        attackGained = attackGained * 1.1;
    end

    effect.SetMagnitude(attackGained);
    effect.SetExtra(defenseLost);

    owner.AddMod(modifiersGlobal.Attack, effect.GetMagnitude());
    owner.SubtractMod(modifiersGlobal.Defense, effect.GetExtra());
end

function onCommandStart(effect, owner, command, actionContainer)
    --if command is a weaponskill or jump
    --27266: jump
    if command.GetCommandType() == CommandType.Weaponskill or command.id == 27266 then
        --At III just refresh the effect
        effect.RefreshTime();
    end
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Attack, effect.GetMagnitude());
    owner.AddMod(modifiersGlobal.Defense, effect.GetExtra());
end