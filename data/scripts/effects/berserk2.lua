require("modifiers");

function onGain(owner, effect, actionContainer)
end

--Increases attack power and reduces defense with each successful attack
--Does this include weaponskills?
--Is this on every hit or every succesfull skill useage?
function onHit(effect, attacker, defender, skill, action, actionContainer)
    --Trait increases effect by 20%. Does this include the reduced defense, 
    --does this increase the cap or the rate at which you get AP or both?

    if (effect.GetExtra() < 10) then
        --This will count how many hits there have been
        effect.SetExtra(effect.GetExtra() + 1);

        --If you update these make sure to update them in Whirlwind as well
        local apPerHit = 20;
        local defPerHit = 20;

        if effect.GetTier() == 2 then
            apPerHit = 24;
        end

        --Just going to say every hit adds 20 AP up to 200
        --Same for defense
        --Traited will be 24 up to 240
        --assuming defense is static
        attacker.AddMod(modifiersGlobal.Attack, apPerHit);
        attacker.SubtractMod(modifiersGlobal.Defense, defPerHit);
    end
end;

function onDamageTaken(effect, attacker, defender, skill, action, actionContainer)
    local apPerHit = 20;
    local defPerHit = 20;
    
    if effect.GetTier() == 2 then
        apPerHit = 24;
    end

    defender.SubtractMod(modifiersGlobal.Attack, effect.GetExtra() * apPerHit);
    defender.SubtractMod(modifiersGlobal.Defense, effect.GetExtra() * defPerHit);
    effect.SetExtra(0);
end

function onLose(owner, effect, actionContainer)
    local apPerHit = 20;
    local defPerHit = 20;
    
    if effect.GetTier() == 2 then
        apPerHit = 24;
    end

    owner.SubtractMod(modifiersGlobal.Attack, effect.GetExtra() * apPerHit);
    owner.SubtractMod(modifiersGlobal.Defense, effect.GetExtra() * defPerHit);
end