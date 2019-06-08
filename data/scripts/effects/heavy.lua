require("modifiers")

function onGain(owner, effect, actionContainer)
    local speedModifier = 0.8;

    owner.MultiplyMod(modifiersGlobal.MovementSpeed, speedModifier);
end;

function onLose(owner, effect, actionContainer)    
    local speedModifier = 0.8;

    owner.DivideMod(modifiersGlobal.MovementSpeed, speedModifier);
end;