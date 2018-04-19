require("modifiers")

--Rampart gives 105 defense at level 50.
--Guessing it scales with level. If I had to guess it's either 2.1 * level or (2 * level) + 5. 
--I'm going to guess the latter since it always leaves you with a whole number. I could be completely wrong though
--The party_battle_leve has rampart giving 36? defense. It's from an earlier patch so probably useless
function onGain(target, effect)
    effect.SetMagnitude(2 * target.GetLevel() + 5);

    target.AddMod(modifiersGlobal.Defense, effect.GetMagnitude());
end;

function onLose(target, effect)
    target.SubtractMod(modifiersGlobal.Defense, effect.GetMagnitude());
end;