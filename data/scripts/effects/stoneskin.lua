require("global")
require("utils")
require("modifiers")
require("hiteffect")
require("battleutils")


--todo: calculate actual mitigation value based on Source's enhancing magic. info: http://forum.square-enix.com/ffxiv/threads/40800-Enhancing-Magic
--This should also probably be calculated when the spell is cast so it doesnt overwrite a stronger stoneskin
function onGain(owner, effect)
    --Going to assume its 1.34 * Enhancing Potency untraited, 1.96 * Enhancing Potency traited.
    local potencyModifier = 1.34;
    if effect.tier == 2 then
        potencyModifier = 1.96;
    end
    local amount = potencyModifier * effect.source.GetMod(modifiersGlobal.MagicEnhancePotency);

    owner.AddMod(modifiersGlobal.Stoneskin, amount);
end

--Using extra for how much mitigation stoneskin has
function onPostAction(owner, effect, caster, skill, action, actionContainer)
    if (owner.GetMod(modifiersGlobal.Stoneskin) <= 0) then
        actionContainer.AddAction(owner.statusEffects.RemoveStatusEffectForBattleAction(effect));
    end
end;