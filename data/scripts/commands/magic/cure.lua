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

    --Non-CNJ
    --1.10 per HMP
    --0 per MND    
    local hpPerHMP = 1.10;
    local hpPerMND = 0;

    --CNJ
    --With AF:
    --1.25 HP per Healing Magic Potency
    --0.25 HP per MND
    --This is WITH AF chest. Without is lower. AF is ~7-10% increase apparently
    --I'm guessing without AF hpPerHMP will be 1.1?
    if (caster.GetClass() == 23) then
        hpPerHMP = 1.25;
        hpPerMND = 0.25;
    end

    action.amount = hpPerHMP * caster.GetMod(modifiersGlobal.HealingMagicPotency) + hpPerMND * (caster.GetMod(modifiersGlobal.Mind));

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;