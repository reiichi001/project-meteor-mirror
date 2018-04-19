function onGain(owner, effect)
    --Using extra because that's what blissful_mind uses
    effect.SetExtra(effect.GetMagnitude());
end

function onTick(owner, effect)
    print("hi")
end

function onLose(owner, effect)
end
