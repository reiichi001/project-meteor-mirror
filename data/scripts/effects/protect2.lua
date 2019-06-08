require("modifiers")

function onGain(owner, effect, actionContainer)
    --Magnitude is caster's Enhancing Magic Potency.    
    --http://forum.square-enix.com/ffxiv/threads/41900-White-Mage-A-Guide
    --5-4-5-4-5-4-5-4-5 repeating points of Enhancing for 1 defense
    --4.56 * Enhancing Potency
    local defenseBuff = 4.56 * effect.GetMagnitude();
    local magicDefenseBuff = 0;

    owner.AddMod(modifiersGlobal.Defense, defenseBuff);

    --27365: Enhanced Protect: Increases magic defense gained from Protect.
    --There is no "magic defense" stat, instead it gives stats to each resist stat.
    magicDefenseBuff = 6.67 * effect.GetMagnitude();
    for i = modifiersGlobal.FireResistance, modifiersGlobal.WaterResistance do
        owner.AddMod(i, magicDefenseBuff);
    end
    

end;

function onLose(owner, effect, actionContainer)    
    local defenseBuff = 4.56 * effect.GetMagnitude();
    local magicDefenseBuff = 0;

    owner.SubtractMod(modifiersGlobal.Defense, defenseBuff);

    --27365: Enhanced Protect: Increases magic defense gained from Protect.
    --There is no "magic defense" stat, instead it gives stats to each resist stat.
    magicDefenseBuff = 6.67 * effect.GetMagnitude();
    for i = modifiersGlobal.FireResistance, modifiersGlobal.WaterResistance do
        owner.SubtractMod(i, magicDefenseBuff);
    end
end;

