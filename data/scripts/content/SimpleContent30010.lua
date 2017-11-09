require ("global")
require ("ally")
require ("modifiers")

function onCreate(starterPlayer, contentArea, director)
	--papalymo = contentArea:SpawnActor(2290005, "papalymo", 365.89, 4.0943, -706.72, -0.718);
	--yda = contentArea:SpawnActor(2290006, "yda", 365.266, 4.122, -700.73, 1.5659);	

	--mob1 = contentArea:SpawnActor(2201407, "mob1", 374.427, 4.4, -698.711, -1.942);
	--mob2 = contentArea:SpawnActor(2201407, "mob2", 375.377, 4.4, -700.247, -1.992);
	--mob3 = contentArea:SpawnActor(2201407, "mob3", 375.125, 4.4, -703.591, -1.54);
    yda = GetWorldManager().SpawnBattleNpcById(6, contentArea);
    papalymo = GetWorldManager().SpawnBattleNpcById(7, contentArea);
    yda:ChangeState(2);
    mob1 = GetWorldManager().SpawnBattleNpcById(3, contentArea);
    mob2 = GetWorldManager().SpawnBattleNpcById(4, contentArea);
    mob3 = GetWorldManager().SpawnBattleNpcById(5, contentArea);
    starterPlayer.currentParty.members:Add(yda.actorId);
	starterPlayer.currentParty.members:Add(papalymo.actorId);
	starterPlayer:SetMod(modifiersGlobal.MinimumHpLock, 1);
	
	
	openingStoper = contentArea:SpawnActor(1090384, "openingstoper", 356.09, 3.74, -701.62, -1.41);
	
	director:AddMember(starterPlayer);
	director:AddMember(director);
 	director:AddMember(papalymo);
	director:AddMember(yda);
	director:AddMember(mob1);
	director:AddMember(mob2);
	director:AddMember(mob3);
	
	--director:StartContentGroup();
	
end

function onUpdate(area, tick)
	local players = area:GetPlayers()
	local mobs = area:GetMonsters()
	local allies = area:GetAllies()
	local resumeChecks = true
	for player in players do
		if player then
			local exitLoop = false
			for ally in allies do
				if ally then
					if not ally:IsEngaged() then
						if player:IsEngaged() then
							ally.neutral = false
							ally.isAutoAttackEnabled = true
							ally:SetMod(modifiersGlobal.Speed, 8)
							allyGlobal.EngageTarget(ally, player.target)
							exitLoop = true
							break
						-- todo: support scripted paths
						elseif ally:GetSpeed() > 0 then
							
						end
					end
				end
			end
			if exitLoop then
				resumeChecks = false
				break
			end
		end
	end
	if not resumeChecks then
		return
	end
end

function onDestroy()

	

end