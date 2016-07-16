properties = {
    permissions = 0,
    parameters = "s",
    description = "plays music <id> to player",
}

function onTrigger(player, argc, music)
    music = tonumber(music) or 0;
    player:ChangeMusic(music);
end;