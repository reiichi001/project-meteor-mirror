require("modifiers")

function onGain(owner, effect)

    owner.AddMod(modifiersGlobal.Stoneskin, effect.GetMagnitude());
end

--Using extra for how much mitigation stoneskin has
function onPostAction(caster, target, effect, skill, action, actionContainer)
    if (owner.GetMod(modifiersGlobal.Stoneskin) <= 0) then
        actionContainer.AddAction(owner.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end;