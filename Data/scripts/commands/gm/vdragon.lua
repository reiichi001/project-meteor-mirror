require("global");
require("utils");

properties = {
    permissions = 0,
    parameters = "sssss",
    description =
[[
Angle stuff!
!anglestuff
]],
}

function onTrigger(player, argc)
    local sender = "[battleaction] ";

    if player and player.currentTarget then
        local actor = GetWorldManager():GetActorInWorld(player.currentTarget) or nil;
        actor.Ability(23459, actor.actorId);

    else
        print(sender.."unable to add experience, ensure player name is valid.");
    end;
end;