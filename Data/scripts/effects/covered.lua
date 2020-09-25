require("battleUtils")

--If person who cast cover is within 8y, change the action's target to them
--Not sure what attacks are valid. It only says "melee attacks", unsure if that includes weaponskills and abilities or just auto attacks
--Right now this will probably be really buggy, since Covered is not necessarily the first effect that will activate on the target

--Really need to do more research on this, figure out how it interacts with multi-hit attacks, aoe attacks (if those count as melee ever?), etc.
--If it redirects the whole attack instead of a single hit, we might need a new that activates while iterating over targets.
function onPreAction(effect, caster, target, skill, action, actionContainer)
    if not skill.isRanged then
        if effect.GetSource().IsAlive() and getDistanceBetweenActors(effect.GetSource(), target) <= 8 then
            target = effect.GetSource();
        end
    end
end;