require("modifiers");

--15% in ARR, dont know how it worked in 1.0
function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.RawEvadeRate, 15);
end;

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.RawEvadeRate, 15);
end;

--Returns 25%? of amount dodged as MP
function onEvade(effect, attacker, defender, skill, action, actionContainer)
    --25% of amount dodged untraited, 50% traited
    local percent = 0.25;
    if (effect.GetTier() == 2) then
        percent = 0.50;
    end

    local mpToReturn = percent * action.amountMitigated;
    defender.AddMP(math.ceil(mpToReturn));
    --33010: You recover x MP from Featherfoot
    actionContainer.AddMPAction(defender.actorId, 33010, mpToReturn);
    --Featherfoot is lost after evading
    defender.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
end;