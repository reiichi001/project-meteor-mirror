require("modifiers")

--Battle Voice grants HP_Boost and it sets max hp to 125% normal amount and heals for the difference between current
--This doesn't seem like the correct way to do this. If max HP changes between gainign and losing wont this break?
function onGain(owner, effect, actionContainer)
    local newMaxHP = owner.GetMaxHP() * 1.25;
    local healAmount = newMaxHP - owner.GetMaxHP();

    owner.SetMaxHP(newMaxHP);
    owner.AddHP(healAmount);
end;

function onLose(owner, effect, actionContainer)
    owner.SetMaxHP(owner.GetMaxHP() / 1.25);
end;