require("modifiers")

function onGain(owner, effect, actionContainer)
    local speedModifier = 1.25;

    owner.MultiplyMod(modifiersGlobal.MovementSpeed, speedModifier);
end;

function onLose(owner, effect, actionContainer)    
    local speedModifier = 1.25;

    owner.DivideMod(modifiersGlobal.MovementSpeed, speedModifier);
end;