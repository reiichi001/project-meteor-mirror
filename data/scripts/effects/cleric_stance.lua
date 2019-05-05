require("modifiers")

function onGain(target, effect)
    --Multiples Attack Magic Potency by 1.2 and Healing Magic Potency by 0.8
    target.SetMod(modifiersGlobal.MagicAttack, target.GetMod(modifiersGlobal.MagicAttack) * 1.2);
    target.SetMod(modifiersGlobal.MagicHeal, target.GetMod(modifiersGlobal.MagicHeal) * 0.8);
end;

function onLose(target, effect)
    target.SetMod(modifiersGlobal.MagicAttack, target.GetMod(modifiersGlobal.MagicAttack) / 1.2);
    target.SetMod(modifiersGlobal.MagicHeal, target.GetMod(modifiersGlobal.MagicHeal) / 0.8);
end;