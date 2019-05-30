require("global");
require("ability");
require("battleutils")

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)

    --27163: Enhanced Rampart:Expands rampart to affect party members
    if caster.HasTrait(27163) then
        ability.aoeType = TargetFindAOEType.Circle;
    end

    return 0;
end;

--http://forum.square-enix.com/ffxiv/threads/47393-Tachi-s-Guide-to-Paladin-%28post-1.22b%29
--180 enmity per member that has enmity on the current enemy
--Need to figure out enmity system
function onSkillFinish(caster, target, skill, action, actionContainer)
    action.enmity = 180;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;