require("modifiers")

function onGain(owner, effect, actionContainer)
    --Multiples Attack Magic Potency by 1.2 and Healing Magic Potency by 0.8
    owner.MultiplyMod(modifiersGlobal.AttackMagicPotency, 1.2);
    owner.MultiplyMod(modifiersGlobal.HealingMagicPotency, 0.8);
end;

function onLose(owner, effect, actionContainer)
    owner.DivideMod(modifiersGlobal.AttackMagicPotency, 1.2);
    owner.DivideMod(modifiersGlobal.HealingMagicPotency, 0.8);
end;