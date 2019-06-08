function onGain(owner, effect, actionContainer)
    --Using extra because that's what blissful_mind uses
    effect.SetExtra(effect.GetMagnitude());
end

function onLose(owner, effect, actionContainer)
end
