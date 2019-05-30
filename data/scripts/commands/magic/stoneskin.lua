require("global");
require("magic");
require("modifiers")

function onMagicPrepare(caster, target, spell)
    return 0;
end;

function onMagicStart(caster, target, spell)
    return 0;
end;

--http://forum.square-enix.com/ffxiv/threads/41900-White-Mage-A-Guide
function onSkillFinish(caster, target, skill, action, actionContainer)

    local hpPerPoint = 1.34;--? 1.33?

    --27364: Enhanced Stoneskin: Increases efficacy of Stoneskin
    if caster.HasTrait(27364) then
        hpPerPoint = 1.96;
    end

    skill.statusMagnitude = hpPerPoint * caster.GetMod(modifiersGlobal.EnhancementMagicPotency);

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;