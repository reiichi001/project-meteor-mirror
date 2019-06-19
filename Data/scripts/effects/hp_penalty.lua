require("modifiers")

function onGain(owner, effect, actionContainer)
    local newMaxHP = owner.GetMaxHP() * 0.75;

    owner.SetMaxHP(newMaxHP);
end;

function onLose(owner, effect, actionContainer)
    owner.SetMaxHP(owner.GetMaxHP() / 0.75);
end;