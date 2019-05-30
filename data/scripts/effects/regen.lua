--Regen is modified by Enhancing Magic Potency. Formula here: http://forum.square-enix.com/ffxiv/threads/41900-White-Mage-A-Guide
function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.Regen, effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Regen, effect.GetMagnitude());
end
