
function onCreate(starterPlayer, contentArea, director)
	
	niellefresne = contentArea:SpawnActor(2290003, "niellefresne", -11.86, 192, 35.06, -0.8);
	thancred = contentArea:SpawnActor(2290004, "thancred", -26.41, 192, 39.52, 1.2);
	thancred:ChangeState(2);
	
	mob1 = contentArea:SpawnActor(2203301, "mob1", -6.193, 192, 47.658, -2.224);

	openingStoper = contentArea:SpawnActor(1090385, "openingstoper", -24.34, 192, 34.22, 0);
	
	director:AddMember(starterPlayer);
	director:AddMember(director);
	director:AddMember(niellefresne);
	director:AddMember(thancred);
	director:AddMember(mob1);
	
	director:StartContentGroup();
	
end

function onDestroy()

	

end