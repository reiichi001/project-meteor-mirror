require("global")
require("utils")
require("modifiers")
require("hiteffect")
require("battleutils")

function onGain(owner, effect)

    owner.AddMod(modifiersGlobal.Stoneskin, effect.GetMagnitude());
end

--Using extra for how much mitigation stoneskin has
function onPostAction(owner, effect, caster, skill, action, actionContainer)
    if (owner.GetMod(modifiersGlobal.Stoneskin) <= 0) then
        actionContainer.AddAction(owner.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end;