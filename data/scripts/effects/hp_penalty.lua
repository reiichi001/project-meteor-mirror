require("modifiers")

function onGain(target, effect)
    local newMaxHP = target.GetMaxHP() * 0.75;

    target.SetMaxHP(newMaxHP);
end;

function onLose(target, effect)
    target.SetMaxHP(target.GetMaxHP() / 0.75);
end;