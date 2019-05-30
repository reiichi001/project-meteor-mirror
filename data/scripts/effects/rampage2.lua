require("modifiers")
require("battleutils")
require("utils")

parryPerDT = 20;
delayMsPerDT = 100;

function onGain(owner, effect, actionContainer)
end

--Increases parry rating and attack speed for each hit. (Need more info)
function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    --Assuming 20 parry rating every time you're hit up to 200
    --Delay is more complicated. Most axes are around 4 seconds, so i'm gonna assume it cuts off a full second at max
    if (effect.GetExtra() < 10) then
        effect.SetExtra(effect.GetExtra() + 1);

        defender.AddMod(modifiersGlobal.Parry, parryPerDT);
        defender.SubtractMod(modifiersGlobal.Delay, delayMsPerDT);
    end
end

--Heals for 50% of damage dealt on crits with a maximum of 20% of max hp
--Also only heals for as much hp as you're missing at most
function onCrit(effect, attacker, defender, skill, action, actionContainer)
    local healAmount = math.Clamp(action.amount * 0.50, 0, attacker.GetMaxHP() * 0.20);
    healAmount = math.Clamp(healAmount, 0, attacker.GetMaxHP() - attacker.GetHP());
    attacker.AddHP(healAmount);
    --33012: You recover [healAmount] HP.
    actionContainer.AddHPAbsorbAction(defender.actorId, 33008, healAmount);
end;

--"Effect fades over time"
--Rampage ticks every 6 seconds
function onTick(owner, effect, actionContainer)
    --Enduring march prevents fading of rampage effect
    if not owner.statusEffects.HasStatusEffect(223078) and (effect.GetExtra() > 0) then
        owner.SubtractMod(modifiersGlobal.Parry, parryPerDT);
        owner.AddMod(modifiersGlobal.Delay, delayMsPerDT);
        effect.SetExtra(effect.GetExtra() - 1);
    end
end

function onLose(owner, effect, actionContainer)
    owner.SubtractMod(modifiersGlobal.Parry, effect.GetExtra() * parryPerDT);
    owner.AddMod(modifiersGlobal.Delay, effect.GetExtra() * delayMsPerDT);
end