require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

function onCombo(caster, target, skill)
    --http://forum.square-enix.com/ffxiv/threads/50479-Gladiator-Paladin-STR-MND-Stat-Caps/page7
    --4.5 is a bonus on top of the 1x of normal flat blade
    --This is modified by MND and dlvl and caps at 4.5, dont know the values used though
    skill.enmityModifier = 5.5;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);
end;