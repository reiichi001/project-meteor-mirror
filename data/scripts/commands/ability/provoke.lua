require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27200: Enhanced Provoke: Adds Attack Down effect to Provoke.
    if caster.HasTrait(27200) then
        ability.statusChance = 1.0;
    end
    return 0;
end;

--http://forum.square-enix.com/ffxiv/threads/47393-Tachi-s-Guide-to-Paladin-%28post-1.22b%29
function onSkillFinish(caster, target, skill, action, actionContainer)
    action.enmity = 750;
    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;