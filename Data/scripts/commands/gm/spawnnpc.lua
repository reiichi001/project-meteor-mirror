require("global");
require("modifiers");
properties = {
    permissions = 0,
    parameters = "sss",
    description = 
[[
yolo
]],
}

local modelIds = 
{
    ["titan"] = 2107401, 
	["ifrit"] = 2207302,
	["ifrithotair"] = 2207310,
    ["nail"] = 2207307, 
	["garuda"] = 2209501, 
	["garudahelper"] = 2209516,
    ["plume"] = 2209502, 
    ["monolith"] = 2209506, 
    ["mog"] = 2210408, 
    ["nael"] = 2210902, 
    ["meteor"] = 2210903,
	["cactuar"] = 2200905, 
	["morbol"] = 2201002, 
	["drake"] = 2202209, 
	["ogre"] = 2202502, 
	["treant"] = 2202801, 
	["couerl"] = 2203203, 
	["wyvern"] = 2203801, 
	["clouddragon"] = 2208101, 
	["golem"] = 2208901, 
	["atomos"] = 2111002, 
	["chimera"] = 2308701, 
	["salamander"] = 2201302, 
	["ahriman"] = 2201704, 
	["rat"] = 9111275, 
	["bat"] = 2104113, 
	["chigoe"] = 2105613, 
	["hedgemole"] = 2105709, 
	["gnat"] = 2200604, 
	["bird"] = 2201208, 
	["puk"] = 2200112, 
	["angler"] = 2204507, 
	["snurble"] = 2204403, 
	["lemur"] = 2200505, 
	["doe"] = 2200303, 
	["hippogryph"] = 2200405, 
	["trap"] = 2202710, 
	["goat"] = 2102312, 
	["dodo"] = 9111263, 
	["imp"] = 2202607, 
	["spriggan"] = 2290036, 
	["cyclops"] = 2210701, 
	["raptor"] = 2200205, 
	["wolf"] = 2201429, 
	["fungus"] = 2205907, 
	["basilisk"] = 2200708, 
	["bomb"] = 2201611, 
	["jellyfish"] = 2105415, 
	["slug"] = 2104205, 
	["coblyn"] = 2202103, 
	["ghost"] = 2204317, 
	["crab"] = 2107613, 
	["yarzon"] = 2205520, 
	["elemental"] = 2105104, 
	["boar"] = 2201505, 
	["kobold"] = 2206629, 
	["sylph"] = 2206702, 
	["ixal"] = 2206434, 
	["amaljaa"] = 2206502, 
	["qiqirn"] = 2206304, 
	["apkallu"] = 2202902, 
	["goobbue"] = 2103301, 
	["garlean"] = 2207005, 
	["flan"] = 2103404, 
    ["swarm"] = 2105304,
    ["goblin"] =  2210301,
    ["buffalo"] =  2200802,
    ["skeleton"] = 2201902,
    ["zombie"] = 2201807,
    ["toad"] = 2203107,
    ["wisp"] = 2209903,
    ["juggernaut"] = 6000243,
    ["mammet"] = 6000246,
    ["lantern"] = 1200329,
    ["helper"] = 2310605,
    ["diremite"] = 2101108,
    ["gong"] = 1200050,
}

function onTrigger(player, argc, name,  width, height, blockCount)
    local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
    local sender = "spawnnpc";
	
	if player and (modelIds[name] != nil) then
        local pos = player:GetPos();
        local x = tonumber(pos[0]);
        local y = tonumber(pos[1]);
        local z = tonumber(pos[2]);
        local rot = tonumber(pos[3]);
        local zone = pos[4];
        local w = tonumber(width) or 0;

        local h = tonumber(height) or 0;
        local blocks = tonumber(blockCount) or 0;
		for b = 0, blocks do
            for i = 0, w do
                for j = 0, h do
                    local actor = player.GetZone().SpawnActor(2104001, 'ass', x + (i * 1), y, z + (j * 1), rot, 0, 0, true);
                    actor.ChangeNpcAppearance(modelIds[name]);
                    actor.SetMaxHP(5000);
                    actor.SetHP(5000);
                    actor.SetMod(modifiersGlobal.CanBlock, 1);
                    actor.SetMod(modifiersGlobal.AttackRange, 3);
                    actor.SetMod(modifiersGlobal.MovementSpeed, 5);
                    actor.SetMobMod(mobModifiersGlobal.Roams, 1);
					actor.SetMobMod(mobModifiersGlobal.RoamDelay, 10);
					actor.charaWork.parameterSave.state_mainSkillLevel = 52;
                    actor.moveState = 3;
                end;
            end;

            x = x + 500
        end;
        return;
	elseif player and (modelIds[name] == nil) then
		player:SendMessage(messageID, sender, "That name isn't valid");
	else
		print("I don't even know how you managed this")
	end

	return;
end;