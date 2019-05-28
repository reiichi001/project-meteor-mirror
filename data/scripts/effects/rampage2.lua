require("modifiers")
require("utils")

parryPerDT = 20;
delayMsPerDT = 100;

function onGain(owner, effect)
    owner.statusEffects.RemoveStatusEffect(223207);
end

--Increases parry rating and attack speed for each hit. (Need more info)
function onDamageTaken(effect, attacker, defender, action, actionContainer)
    
    --Assuming 20 parry rating every time you're hit up to 200
    --Delay is more complicated. Most axes are around 4 seconds, so i'm gonna assume it cuts off a full second at max
    if (effect.GetExtra() < 10) then
        effect.SetExtra(effect.GetExtra() + 1);

        attacker.AddMod(modifiersGlobal.Parry, parryPerDT);
        attacker.SubtractMod(modifiersGlobal.AttackDelay, delayMsPerDT);
    end
end

--Heals for 50% of damage dealt on crits with a maximum of 20% of max hp
--Also only heals for as much hp as you're missing at most
function onCrit(effect, attacker, defender, action, actionContainer)
    local healAmount = math.Clamp(action.amount * 0.50, 0, defender.GetMaxHP() * 0.20);
    healAmount = math.Clamp(healAmount, 0, defender.GetMaxHP() - defender.GetHP());
    defender.AddHP(healAmount);
    --33012: You recover [healAmount] HP.
    actionContainer.AddHPAction(owner.actorId, 33008, healAmount);
end;

--"Effect fades over time"
function onTick(owner, effect)
    --Enduring march prevents fading of rampage effect
    if not owner.statusEffects.HasStatusEffect(223078) and (effect.GetExtra() > 0) then
        --Going to assume that every 5 seconds a single hits worth of rampage is lost.
        owner.SubtractMod(modifiersGlobal.Parry, parryPerDT);
        owner.AddMod(modifiersGlobal.Delay, delayMsPerDT);
        effect.SetExtra(effect.GetExtra() - 1);
    end
end

function onLose(owner, effect)
    owner.SubtractMod(modifiersGlobal.Parry, effect.GetExtra() * parryPerDT);
    owner.AddMod(modifiersGlobal.Delay, effect.GetExtra() * delayMsPerDT);
end