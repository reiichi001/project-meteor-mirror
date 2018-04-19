--Consistent 85HP/tick normal; 113HP/tick with AF pants
function onGain(owner, effect)
    local magnitude = 85

    --Need a better way to set magnitude when adding effects
    if effect.GetTier() == 2 then
        magnitude = 113;
    end
    effect.SetMagnitude(magnitude);

    owner.AddMod(modifiersGlobal.Regen, effect.GetMagnitude());
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Regen, effect.GetMagnitude());
end
