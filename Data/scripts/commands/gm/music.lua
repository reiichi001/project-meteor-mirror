properties = {
    permissions = 0,
    parameters = "s",
    description =
[[
Plays music <id> to player.
!music <id>
]],
}

function onTrigger(player, argc, music)
    music = tonumber(music) or 0;
    player:ChangeMusic(music);
end;