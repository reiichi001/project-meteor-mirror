require("global");
require("weaponskill");
require("modifiers")

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    --Get Berserk statuseffect
    local berserk = caster.statusEffects.GetStatusEffectById(223160);

    --if it isn't nil, remove the AP and Defense mods and reset extra to 0, increase accuracy
    if berserk != nil then
        local apPerHit = 20;
        local defPerHit = 20;

        if berserk.GetTier() == 2 then
            apPerHit = 24;
        end

        caster.SubtractMod(modifiersGlobal.Attack, apPerHit * berserk.GetExtra());
        caster.Add(modifiersGlobal.Defense, defPerHit * berserk.GetExtra());

        berserk.SetExtra(0);
        skill.accuracyModifier = 50;
    end;

    return 0;
end;

--Increased crit rate
function onCombo(caster, target, skill)
    --This is about 25% crit. Don't know if that's what it gave on retail but seems kind of reasonable
    skill.critRateBonus = 200;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;