require("global");
require("magic");

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    --27363: Enhanced Raise: No longer inflicts weakness.
    if caster.HasTrait(27363) then
        ability.statusId = 0;
    end
    return 0;
end;

--Not sure how raise works yet.
function onSkillFinish(caster, target, skill, action, actionContainer)
    action.DoAction(caster, target, skill, actionContainer)
end;