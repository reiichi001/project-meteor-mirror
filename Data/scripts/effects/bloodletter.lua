require("modifiers")

--This is the comboed version of bloodletter.
--All videos I can find have it dealing 15 damage.
--Damage type is projectile.
--DoT damage is combined and all hits on a single tick. ie a blodletter doing 15 damage and aero doing 11 will combine and tick for 26.

--Bloodletter is apparently impacted by PIE
--http://forum.square-enix.com/ffxiv/threads/35795-STR-DEX-PIE-ATK-Testing/page2
--Chance to land is also impacted by PIE
--This is because PIE increases Enfeebling Magic Potency which impacts additional effect damage and land rates
function onGain(owner, effect, actionContainer)
    owner.AddMod(modifiersGlobal.RegenDown, 15);
end

--Additional damage is 570 at level 50
--https://ffxiv.gamerescape.com/w/index.php?title=Bloodletter&oldid=298020
function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.RegenDown, 15);
    owner.DelHP(570, actionContainer);
end
