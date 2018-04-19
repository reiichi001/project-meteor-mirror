require("global");
require("ability");

function onAbilityPrepare(caster, target, ability)
    return 0;
end;

function onAbilityStart(caster, target, ability)
    --27240: Enhanced Hawks Eye
    --Increases accuracy gained by 50%. (Hawks Eye normally gives 12.5% of your accuracy, Traited it gives 18.75%)
    if caster.HasTrait(27240) then
        ability.statusTier = 2
    end
    return 0;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    action.DoAction(caster, target, skill, actionContainer);
end;