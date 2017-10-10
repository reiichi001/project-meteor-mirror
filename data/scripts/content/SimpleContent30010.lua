require ("global")

function onCreate(starterPlayer, contentArea, director)
	--papalymo = contentArea:SpawnActor(2290005, "papalymo", 365.89, 4.0943, -706.72, -0.718);
	--yda = contentArea:SpawnActor(2290006, "yda", 365.266, 4.122, -700.73, 1.5659);	
	--yda = GetWorldManager().SpawnBattleNpcById(6, contentArea);
    --papalymo = GetWorldManager().SpawnBattleNpcById(7, contentArea);
    
    --mob1 = GetWorldManager().SpawnBattleNpcById(3, contentArea);
    --mob2 = GetWorldManager().SpawnBattleNpcById(4, contentArea);
    --mob3 = GetWorldManager().SpawnBattleNpcById(5, contentArea);
	---yda:ChangeState(2);
	
	--mob1 = contentArea:SpawnActor(2201407, "mob1", 374.427, 4.4, -698.711, -1.942);
	--mob2 = contentArea:SpawnActor(2201407, "mob2", 375.377, 4.4, -700.247, -1.992);
	--mob3 = contentArea:SpawnActor(2201407, "mob3", 375.125, 4.4, -703.591, -1.54);
	
	--openingStoper = contentArea:SpawnActor(1090384, "openingstoper", 356.09, 3.74, -701.62, -1.41);
	
	--director:AddMember(starterPlayer);
	--director:AddMember(director);
--	director:AddMember(papalymo);
	--director:AddMember(yda);
	--director:AddMember(mob1);
	--director:AddMember(mob2);
	--director:AddMember(mob3);
	
	--director:StartContentGroup();
	
end

function onDestroy()

	

end