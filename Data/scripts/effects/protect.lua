require("modifiers")

function onGain(owner, effect, actionContainer)
    --Magnitude is caster's Enhancing Magic Potency.    
    --http://forum.square-enix.com/ffxiv/threads/41900-White-Mage-A-Guide
    --5-4-5-4-5-4-5-4-5 repeating points of Enhancing for 1 defense
    --4.56 * Enhancing Potency
    local defenseBuff = 4.56 * effect.GetMagnitude();

    owner.AddMod(modifiersGlobal.Defense, defenseBuff);
end;

function onLose(owner, effect, actionContainer)    
    local defenseBuff = 4.56 * effect.GetMagnitude();

    owner.SubtractMod(modifiersGlobal.Defense, defenseBuff);
end;

