require("global");

properties = {
    permissions = 0,
    parameters = "ss",
    description =
[[
Positions your character forward a set <distance>, defaults to 5 yalms.
!nudge |
!nudge <distance> |
!nudge <distance> <up/down> |
]],

}

function onTrigger(player, argc)
		worldMaster = GetWorldMaster();
		player:SendGameMessage(player, worldMaster, 34108, 0x20);	
		player:SendGameMessage(player, worldMaster, 50011, 0x20);	

		director = player:GetZone():CreateDirector("Quest/QuestDirectorMan0l001");
		player:AddDirector(director);
		player:SetLoginDirector(director);
		
		player:KickEvent(director, "noticeEvent", true);
		
		GetWorldManager():DoZoneChange(player, 9);
end;
