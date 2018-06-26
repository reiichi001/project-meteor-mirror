--Forces crit on attacks made with axes
function onPreAction(effect, caster, target, skill, action, actionContainer)
    --Assuming "attacks made with axes" means skills specific to MRD/WAR
    if (skill.job == 3 or skill.job == 17) then
        --Set action's crit rate to 100%
        action.critRate = 100.0;
    end
end;

