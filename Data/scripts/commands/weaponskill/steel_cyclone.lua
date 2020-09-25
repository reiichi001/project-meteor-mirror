require("global");
require("weaponskill");
require("modifiers")

function onSkillPrepare(caster, target, skill)
    return 0;
end;

--Resets rampage to increase damage
function onSkillStart(caster, target, skill)
    --Get Rampage statuseffect
    local rampage = caster.statusEffects.GetStatusEffectById(223208);

    --if it isn't nil, remove the AP and Defense mods and reset extra to 0, increase potency
    if rampage != nil then
        local parryPerDT = 20;
        local delayMsPerDT = 100;

        caster.SubtractMod(modifiersGlobal.Parry, parryPerDT * rampage.GetExtra());
        caster.AddMod(modifiersGlobal.Delay, delayMsPerDT * rampage.GetExtra());

        rampage.SetExtra(0);
        skill.basePotency = skill.basePotency * 1.5;
    end;
    return 0;
end;

--Increased critical hit rate
function onCombo(caster, target, skill)
    skill.bonusCritRate = 200;
end;
    
function onSkillFinish(caster, target, skill, action, actionContainer)
    if target.target == caster then
        skill.statusId = 223015
    end;

    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);

    skill.statusId = 0;
end;