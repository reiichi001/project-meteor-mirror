
function onCreate(starterPlayer, contentArea, contentGroup, director)
	
	yshtola = contentArea:SpawnActor(2290001, "yshtola", -8, 16.35, 6, 0.5);
	stahlmann = contentArea:SpawnActor(2290002, "stahlmann", 0, 16.35, 22, 3);
	
	mob1 = contentArea:SpawnActor(2205403, "mob1", -3.02+3, 17.35, 14.24, -2.81);
	mob2 = contentArea:SpawnActor(2205403, "mob2", -3.02, 17.35, 14.24, -2.81);
	mob3 = contentArea:SpawnActor(2205403, "mob3", -3.02-3, 17.35, 14.24, -2.81);
	
	contentGroup:AddMember(starterPlayer);
	contentGroup:AddMember(director);
	contentGroup:AddMember(yshtola);
	contentGroup:AddMember(stahlmann);
	contentGroup:AddMember(mob1);
	contentGroup:AddMember(mob2);
	contentGroup:AddMember(mob3);
	
end

function onDestroy()

	

end