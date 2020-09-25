--[[

PopulaceCompanyGLPublisher Script

xtx_gcRank for GC Rank values

Functions:

talkOutsider()                              - Dialog for no affiliated with GC.  Seems to always read Maelstrom?
talkOfferWelcome(unk1)                      - Errors
askCompanyLeve()                            - Errors
askLeveDetail(unk1, unk2, unk3, unk4, unk5, unk6, unk7, unk8)                             - Errors

eventGLDifficulty()                         - Difficulty window, returns player choice
eventGLStart(leveName, difficulty, unk1)    - leveName from xtx_guildleve

talkAfterOffer()
talkOfferLimit()

finishTalkTurn()                                            - Resets NPC target/facing

eventGLPlay(leveName, guardianFavor, favorCost, difficulty) - Menu for active levequest
eventGLShinpu(guardianFavor, favorCost)                     - Menu to accept favor buff.  evenGLPlay() calls it
eventGLThanks()                                             - Errors

eventGLReward(                      -- Leve reward screen
    guildleveId, 
    clearTime, 
    missionBonus, 
    difficultyBonus, 
    factionNumber, 
    factionBonus, 
    factionCredit, 
    glRewardItem, 
    glRewardNumber, 
    glRewardSubItem, 
    glRewardSubNumber, 
    difficulty
)


--]]

require ("global")

function init(npc)
	return false, false, 0, 0;	
end

gcOfficer = { 
[1500222] = 1, -- Storm Sergeant Hammil 
[1500223] = 1, -- Storm Sergeant Sternn 
[1500224] = 2, -- Serpent Sergeant Cordwyk 
[1500225] = 2, -- Serpent Sergeant Lodall  
[1500226] = 3, -- Flame Sergeant Byrne  
[1500227] = 3, -- Flame Sergeant Dalvag
}

function onEventStarted(player, npc, triggerName)

    callClientFunction(player, "talkOutsider");

    callClientFunction(player, "finishTalkTurn");
    player:endEvent();
end

