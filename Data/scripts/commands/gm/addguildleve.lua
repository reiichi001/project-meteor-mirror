require("global");

properties = {
    permissions = 0,
    parameters = "s",
    description = "Adds a guildleve by <id>.",
}

function onTrigger(player, argc, glId)    
    if player then
		player:AddGuildleve(tonumber(glId));
	else
        print(sender.."unable to add guildleve, ensure player name is valid.");
    end;
end;