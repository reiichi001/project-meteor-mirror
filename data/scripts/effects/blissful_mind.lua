--The amount of time it takes to fully charge varies depending on how much MP it has to restore, meaning its not just a percent every tick
--Based on a few videos it seems like it heals for 0.5% of max MP every second, traited. This is an early guess but it seems correct
--Untraited is less clear. It could be 0.25%, 0.30%, or 0.40%. Guessing it's 0.30

function onTick(owner, effect, actionContainer)
    local percentPerSecond = 0.0030;

    if effect.GetTier() == 2 then
        percentPerSecond = 0.005;
    end

    local amount = percentPerSecond * owner.GetMaxMP() + 0.25;
    effect.SetExtra(effect.GetExtra() + amount);
    if effect.GetExtra() >= effect.GetMagnitude() then
        --223242: Fully Blissful Mind
        owner.statusEffects.ReplaceEffect(effect, 223242, 1, effect.GetMagnitude(), 0xffffffff);
        --owner.statusEffects.ReplaceEffect(effect, true);
    end
end