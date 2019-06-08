require("global");
require("magic");
require("modifiers");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --http://forum.square-enix.com/ffxiv/threads/41900-White-Mage-A-Guide
    --2.5 HP per Healing Magic Potency
    --0.5 HP per MND
    --this is WITH WHM AF chest, don't know formula without AF. AF seems to increase healing by 7-10%?
    action.amount = 2.5 * caster.GetMod(modifiersGlobal.HealingMagicPotency) + 0.5 * (caster.GetMod(modifiersGlobal.Mind));

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;