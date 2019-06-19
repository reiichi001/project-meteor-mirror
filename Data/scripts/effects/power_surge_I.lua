require("modifiers")
require("battleutils")

--https://www.bluegartr.com/threads/107403-Stats-and-how-they-work/page22

--Base amount of attack gained is 105, which is multiplied by 1.1 if traited. This is why it gives 231 Attack at level 2
--Unsure why defense is a weird number
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
        effect.SetTier(effect.GetTier() + 1);
        
        --Takes 10 weaponskills/jumps to increase level
        if effect.GetTier() > 1 then
            local action = owner.statusEffects.ReplaceEffect(effect, 223213, 1, 1, 60);
            actionContainer.AddAction(action);
        else
            effect.RefreshTime();
        end
    end
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Attack, effect.GetMagnitude());
    owner.AddMod(modifiersGlobal.Defense, effect.GetExtra());
end