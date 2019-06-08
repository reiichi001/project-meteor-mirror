require("global");
require("magic");
require("modifiers");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--http://forum.square-enix.com/ffxiv/threads/41900-White-Mage-A-Guide
function onSkillFinish(caster, target, skill, action, actionContainer)
    --For every 1-2-2-1-2 (repeating 3x) then 1-2-1-2-2 (repeating 3x) Enhancing magic potency you have, the amount your Regen cures per tic increases by 1.
    --.625 * Enhancing
    local slope = 0.625; 
    local intercept = -110;

    --8051406: Healer's Culottes: Enhances Regen
    if caster.HasItemEquippedInSlot(8051406, 14) then
        --I don't know if the numbers in that thread are completely correct because the AF Regen table has 3 1555s in a row.
        --If we assume that AF boots multiply both static parts of the regenTick equation by 1.25, we get a decently close match to actual numbers
        slope = slope * 1.25;
        intercept = intercept * 1.25;
    end

    local regenTick = (slope * caster.GetMod(modifiersGlobal.EnhancementMagicPotency)) + intercept + 1;

    spell.statusMagnitude = regenTick;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;