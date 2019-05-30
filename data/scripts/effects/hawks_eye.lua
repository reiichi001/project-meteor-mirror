require("modifiers");

--In one capture, hawks eye seemed to give 18.75% additional accuracy (379 to 450)
--The player in this capture was a Dragoon, so this is untraited. 
--Traited Hawk's Eye says it increases Accuracy by 50%.
--This could mean traited hawk's eye gives 28.125% (18.75% * 1.5) or it could mean it gives 68.75% (18.75% + 50%)
--It's also possible that Hawk's Eye gives 15 + 15% accuracy untraited, which would give 450.85, which would be rounded down.
--In that case, traited hawks eye could be 15 + 22.5% or 22.5 + 22.5% or (15 + 15%) * 1.5
function onGain(owner, effect, actionContainer)
    local accuracyMod = 0.1875;

    if effect.GetTier() == 2 then
        accuracyMod = 0.28125;
    end

    local amountGained = accuracyMod * owner.GetMod(modifiersGlobal.Accuracy);
    effect.SetMagnitude(amountGained);
    owner.AddMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
end;

function onLose(owner, effect, actionContainer)

    owner.SubtractMod(modifiersGlobal.Accuracy, effect.GetMagnitude());
end;