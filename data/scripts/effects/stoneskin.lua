require("modifiers")

function onGain(owner, effect, actionContainer)

    owner.AddMod(modifiersGlobal.Stoneskin, effect.GetMagnitude());
end

--This is wrong, need to think of a good way of keeping track of how much stoneskin is left when it falls off.
function onLose(owner, effect, actionContainer)
    owner.SetMod(modifiersGlobal.Stoneskin, 0);
end

--Using extra for how much mitigation stoneskin has
function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    if (defender.GetMod(modifiersGlobal.Stoneskin) <= 0) then
        defender.statusEffects.RemoveStatusEffect(effect, actionContainer, 30331, false);
    end
end;