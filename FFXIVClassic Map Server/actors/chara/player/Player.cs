using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.chara.player;
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.events;
using FFXIVClassic_Map_Server.packets.send.actor.inventory;
using FFXIVClassic_Map_Server.packets.send.Actor.inventory;
using FFXIVClassic_Map_Server.packets.send.events;
using FFXIVClassic_Map_Server.packets.send.list;
using FFXIVClassic_Map_Server.packets.send.login;
using FFXIVClassic_Map_Server.packets.send.player;
using FFXIVClassic_Map_Server.utils;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.Actors
{
    class Player : Character
    {
        public const int CLASSID_PUG = 2;
        public const int CLASSID_GLA = 3;
        public const int CLASSID_MRD = 4;
        public const int CLASSID_ARC = 7;
        public const int CLASSID_LNC = 8;
        public const int CLASSID_THM = 22;
        public const int CLASSID_CNJ = 23;

        public const int CLASSID_CRP = 29;
        public const int CLASSID_BSM = 30;
        public const int CLASSID_ARM = 31;
        public const int CLASSID_GSM = 32;
        public const int CLASSID_LTW = 33;
        public const int CLASSID_WVR = 34;
        public const int CLASSID_ALC = 35;
        public const int CLASSID_CUL = 36;

        public const int CLASSID_MIN = 39;
        public const int CLASSID_BTN = 40;
        public const int CLASSID_FSH = 41;

        public const int MAXSIZE_INVENTORY_NORMAL = 200;
        public const int MAXSIZE_INVENTORY_CURRANCY = 320;
        public const int MAXSIZE_INVENTORY_KEYITEMS = 500;
        public const int MAXSIZE_INVENTORY_LOOT = 10;
        public const int MAXSIZE_INVENTORY_MELDREQUEST = 4;
        public const int MAXSIZE_INVENTORY_BAZAAR = 10;
        public const int MAXSIZE_INVENTORY_EQUIPMENT = 35;

        public const int TIMER_TOTORAK = 0;
        public const int TIMER_DZEMAEL = 1;        
        public const int TIMER_BOWL_OF_EMBERS_HARD = 2;
        public const int TIMER_BOWL_OF_EMBERS = 3;
        public const int TIMER_THORNMARCH = 4;
        public const int TIMER_AURUMVALE = 5;
        public const int TIMER_CUTTERSCRY = 6;
        public const int TIMER_BATTLE_ALEPORT = 7;
        public const int TIMER_BATTLE_HYRSTMILL = 8;
        public const int TIMER_BATTLE_GOLDENBAZAAR = 9;
        public const int TIMER_HOWLING_EYE_HARD = 10;
        public const int TIMER_HOWLING_EYE = 11;
        public const int TIMER_CASTRUM_TOWER = 12;
        public const int TIMER_BOWL_OF_EMBERS_EXTREME = 13;
        public const int TIMER_RIVENROAD = 14;
        public const int TIMER_RIVENROAD_HARD = 15;
        public const int TIMER_BEHEST = 16;
        public const int TIMER_COMPANYBEHEST = 17;
        public const int TIMER_RETURN = 18;
        public const int TIMER_SKIRMISH = 19;

        public static int[] MAXEXP = {570, 700, 880, 1100, 1500, 1800, 2300, 3200, 4300, 5000,                   //Level <= 10
                                     5900, 6800, 7700, 8700, 9700, 11000, 12000, 13000, 15000, 16000,            //Level <= 20
                                     20000, 22000, 23000, 25000, 27000, 29000, 31000, 33000, 35000, 38000,       //Level <= 30
                                     45000, 47000, 50000, 53000, 56000, 59000, 62000, 65000, 68000, 71000,       //Level <= 40
                                     74000, 78000, 81000, 85000, 89000, 92000, 96000, 100000, 100000, 110000};   //Level <= 50

        //Event Related
        public uint currentEventOwner = 0;
        public string currentEventName = "";

        public uint currentCommand = 0;
        public string currentCommandName = "";
        
        public uint eventMenuId = 0;

        //Player Info
        public uint[] timers = new uint[20];
        public ushort currentJob;
        public uint currentTitle;
        public uint playTime;
        public uint lastPlayTimeUpdate;
        public bool isGM = false;
        public bool isZoneChanging = true;

        //Inventory        
        private Dictionary<ushort, Inventory> inventories = new Dictionary<ushort, Inventory>();
        private Equipment equipment;

        //GC Related
        public byte gcCurrent;
        public byte gcRankLimsa;
        public byte gcRankGridania;
        public byte gcRankUldah;

        //Mount Related
        public bool hasChocobo;
        public bool hasGoobbue;
        public byte chocoboAppearance;
        public string chocoboName;
        public byte mountState = 0;        

        public uint achievementPoints;

        //Property Array Request Stuff
        private int lastPosition = 0;
        private int lastStep = 0;

        //Quest Actors (MUST MATCH playerWork.questScenario/questGuildleve)
        public Quest[] questScenario = new Quest[16];
        public Quest[] questGuildleve = new Quest[8];

        public Director currentDirector;

        public PlayerWork playerWork = new PlayerWork();

        public ConnectedPlayer playerSession;

        public Player(ConnectedPlayer cp, uint actorID) : base(actorID)
        {
            playerSession = cp;
            actorName = String.Format("_pc{0:00000000}", actorID);
            className = "Player";
            currentSubState = SetActorStatePacket.SUB_STATE_PLAYER;

            inventories[Inventory.NORMAL] = new Inventory(this, MAXSIZE_INVENTORY_NORMAL, Inventory.NORMAL);
            inventories[Inventory.KEYITEMS] = new Inventory(this, MAXSIZE_INVENTORY_KEYITEMS, Inventory.KEYITEMS);
            inventories[Inventory.CURRENCY] = new Inventory(this, MAXSIZE_INVENTORY_CURRANCY, Inventory.CURRENCY);
            inventories[Inventory.MELDREQUEST] = new Inventory(this, MAXSIZE_INVENTORY_MELDREQUEST, Inventory.MELDREQUEST);
            inventories[Inventory.BAZAAR] = new Inventory(this, MAXSIZE_INVENTORY_BAZAAR, Inventory.BAZAAR);
            inventories[Inventory.LOOT] = new Inventory(this, MAXSIZE_INVENTORY_LOOT, Inventory.LOOT);

            equipment = new Equipment(this, inventories[Inventory.NORMAL], MAXSIZE_INVENTORY_EQUIPMENT, Inventory.EQUIPMENT);

            //Set the Skill level caps of all FFXIV (classes)skills to 50
            for (int i = 0; i < charaWork.battleSave.skillLevelCap.Length; i++)
            {
                if (i != CLASSID_PUG &&
                    i != CLASSID_MRD &&
                    i != CLASSID_GLA &&
                    i != CLASSID_MRD &&
                    i != CLASSID_ARC &&
                    i != CLASSID_LNC &&
                    i != CLASSID_THM &&
                    i != CLASSID_CNJ &&
                    i != CLASSID_CRP &&
                    i != CLASSID_BSM &&
                    i != CLASSID_ARM &&
                    i != CLASSID_GSM &&
                    i != CLASSID_LTW &&
                    i != CLASSID_WVR &&
                    i != CLASSID_ALC &&
                    i != CLASSID_CUL &&
                    i != CLASSID_MIN &&
                    i != CLASSID_BTN &&
                    i != CLASSID_FSH)
                    charaWork.battleSave.skillLevelCap[i] = 0xFF;
                else
                    charaWork.battleSave.skillLevelCap[i] = 50;

            }

            charaWork.property[0] = 1;
            charaWork.property[1] = 1;
            charaWork.property[2] = 1;
            charaWork.property[4] = 1;

            charaWork.command[0] =  0xA0F00000 | 21001;
            charaWork.command[1] =  0xA0F00000 | 21001;

            charaWork.command[2] =  0xA0F00000 | 21002;
            charaWork.command[3] =  0xA0F00000 | 12004;
            charaWork.command[4] =  0xA0F00000 | 21005;
            charaWork.command[5] =  0xA0F00000 | 21006;
            charaWork.command[6] =  0xA0F00000 | 21007;
            charaWork.command[7] =  0xA0F00000 | 12009;
            charaWork.command[8] =  0xA0F00000 | 12010;
            charaWork.command[9] =  0xA0F00000 | 12005;
            charaWork.command[10] = 0xA0F00000 | 12007;
            charaWork.command[11] = 0xA0F00000 | 12011;
            charaWork.command[12] = 0xA0F00000 | 22012;
            charaWork.command[13] = 0xA0F00000 | 22013;
            charaWork.command[14] = 0xA0F00000 | 29497;
            charaWork.command[15] = 0xA0F00000 | 22015;

            charaWork.command[32] = 0xA0F00000 | 27191;
            charaWork.command[33] = 0xA0F00000 | 22302;
            charaWork.command[34] = 0xA0F00000 | 28466;

            charaWork.commandAcquired[27150 - 26000] = true;

            playerWork.questScenarioComplete[110001 - 110001] = true;
            playerWork.questGuildleveComplete[120050 - 120001] = true;

            for (int i = 0; i < charaWork.additionalCommandAcquired.Length; i++ )
                charaWork.additionalCommandAcquired[i] = true;
            
            for (int i = 0; i < charaWork.commandCategory.Length; i++)
                charaWork.commandCategory[i] = 1;

            charaWork.battleTemp.generalParameter[3] = 1;

            charaWork.eventSave.bazaarTax = 5;
            charaWork.battleSave.potencial = 6.6f;

            charaWork.commandCategory[0] = 1;
            charaWork.commandCategory[1] = 1;
            charaWork.commandCategory[32] = 1;
            charaWork.commandCategory[33] = 1;
            charaWork.commandCategory[34] = 1;

            charaWork.parameterSave.commandSlot_compatibility[0] = true;
            charaWork.parameterSave.commandSlot_compatibility[1] = true;
            charaWork.parameterSave.commandSlot_compatibility[32] = true;

            charaWork.commandBorder = 0x20;

            charaWork.parameterTemp.tp = 3000;

            Database.loadPlayerCharacter(this);
            lastPlayTimeUpdate = Utils.UnixTimeStampUTC();
        }
        
        public List<SubPacket> create0x132Packets(uint playerActorId)
        {
            List<SubPacket> packets = new List<SubPacket>();
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0xB, "commandForced"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0xA, "commandDefault"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x6, "commandWeak"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x4, "commandContent"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x6, "commandJudgeMode"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x100, "commandRequest"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x100, "widgetCreate"));
            packets.Add(_0x132Packet.buildPacket(playerActorId, 0x100, "macroRequest"));
            return packets;
        }

        public override SubPacket createScriptBindPacket(uint playerActorId)
        {
            List<LuaParam> lParams;
            if (isMyPlayer(playerActorId))
            {
                if (currentDirector != null)
                    lParams = LuaUtils.createLuaParamList("/Chara/Player/Player_work", false, false, true, currentDirector, true, 0, false, timers, true);
                else
                    lParams = LuaUtils.createLuaParamList("/Chara/Player/Player_work", false, false, false, true, 0, false, timers, true);
            }
            else
                lParams = LuaUtils.createLuaParamList("/Chara/Player/Player_work", false, false, false, false, false, true);
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);
        }        

        public override BasePacket getSpawnPackets(uint playerActorId, uint spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId, 8));
            if (isMyPlayer(playerActorId))
                subpackets.AddRange(create0x132Packets(playerActorId));
            subpackets.Add(createSpeedPacket(playerActorId));
            subpackets.Add(createSpawnPositonPacket(playerActorId, spawnType));
            subpackets.Add(createAppearancePacket(playerActorId));
            subpackets.Add(createNamePacket(playerActorId));
            subpackets.Add(_0xFPacket.buildPacket(playerActorId, playerActorId));
            subpackets.Add(createStatePacket(playerActorId));
            subpackets.Add(createIdleAnimationPacket(playerActorId));
            subpackets.Add(createInitStatusPacket(playerActorId));
            subpackets.Add(createSetActorIconPacket(playerActorId));
            subpackets.Add(createIsZoneingPacket(playerActorId));
            subpackets.AddRange(createPlayerRelatedPackets(playerActorId));
            subpackets.Add(createScriptBindPacket(playerActorId));            
            return BasePacket.createPacket(subpackets, true, false);
        }

        public List<SubPacket> createPlayerRelatedPackets(uint playerActorId)
        {
            List<SubPacket> subpackets = new List<SubPacket>();

            if (gcCurrent != 0)
                subpackets.Add(SetGrandCompanyPacket.buildPacket(actorId, playerActorId, gcCurrent, gcRankLimsa, gcRankGridania, gcRankUldah));

            if (currentTitle != 0)
                subpackets.Add(SetPlayerTitlePacket.buildPacket(actorId, playerActorId, currentTitle));

            if (currentJob != 0)
                subpackets.Add(SetCurrentJobPacket.buildPacket(actorId, playerActorId, currentJob));

            if (isMyPlayer(playerActorId))
            {
                subpackets.Add(_0x196Packet.buildPacket(playerActorId, playerActorId));

                if (hasChocobo && chocoboName != null && !chocoboName.Equals(""))
                {
                    subpackets.Add(SetChocoboNamePacket.buildPacket(actorId, playerActorId, chocoboName));
                    subpackets.Add(SetHasChocoboPacket.buildPacket(playerActorId, hasChocobo));
                }

                if (hasGoobbue)
                    subpackets.Add(SetHasGoobbuePacket.buildPacket(playerActorId, hasGoobbue));

                subpackets.Add(SetAchievementPointsPacket.buildPacket(playerActorId, achievementPoints));
                subpackets.Add(Database.getLatestAchievements(this));
                subpackets.Add(Database.getAchievementsPacket(this));                
            }

            return subpackets;
        }

        public override BasePacket getInitPackets(uint playerActorId)
        {
            ActorPropertyPacketUtil propPacketUtil = new ActorPropertyPacketUtil("/_init", this, playerActorId);
                        
            propPacketUtil.addProperty("charaWork.eventSave.bazaarTax");
            propPacketUtil.addProperty("charaWork.battleSave.potencial");

            //Properties
            for (int i = 0; i < charaWork.property.Length; i++)
            {
                if (charaWork.property[i] != 0)                
                    propPacketUtil.addProperty(String.Format("charaWork.property[{0}]", i));
            }
            
            //Parameters
            propPacketUtil.addProperty("charaWork.parameterSave.hp[0]");
            propPacketUtil.addProperty("charaWork.parameterSave.hpMax[0]");
            propPacketUtil.addProperty("charaWork.parameterSave.mp");
            propPacketUtil.addProperty("charaWork.parameterSave.mpMax");
            propPacketUtil.addProperty("charaWork.parameterTemp.tp");
            propPacketUtil.addProperty("charaWork.parameterSave.state_mainSkill[0]");
            propPacketUtil.addProperty("charaWork.parameterSave.state_mainSkillLevel");
            
            //Status Times
            for (int i = 0; i < charaWork.statusShownTime.Length; i++)
            {
                if (charaWork.statusShownTime[i] != 0xFFFFFFFF)
                    propPacketUtil.addProperty(String.Format("charaWork.statusShownTime[{0}]", i));
            }
        
            //General Parameters
            for (int i = 3; i < charaWork.battleTemp.generalParameter.Length; i++)
            {
                if (charaWork.battleTemp.generalParameter[i] != 0)
                    propPacketUtil.addProperty(String.Format("charaWork.battleTemp.generalParameter[{0}]", i));
            }
            
            propPacketUtil.addProperty("charaWork.battleTemp.castGauge_speed[0]");
            propPacketUtil.addProperty("charaWork.battleTemp.castGauge_speed[1]");
            
            //Battle Save Skillpoint
            
            //Commands
            propPacketUtil.addProperty("charaWork.commandBorder");


            for (int i = 0; i < charaWork.command.Length; i++)
            {
                if (charaWork.command[i] != 0)
                    propPacketUtil.addProperty(String.Format("charaWork.command[{0}]", i));
            }
         
            
            for (int i = 0; i < charaWork.commandCategory.Length; i++)
            {
                charaWork.commandCategory[i] = 1;
                if (charaWork.commandCategory[i] != 0)
                    propPacketUtil.addProperty(String.Format("charaWork.commandCategory[{0}]", i));
            }

            for (int i = 0; i < charaWork.commandAcquired.Length; i++)
            {
                if (charaWork.commandAcquired[i] != false)
                    propPacketUtil.addProperty(String.Format("charaWork.commandAcquired[{0}]", i));
            }
            

            for (int i = 0; i < charaWork.additionalCommandAcquired.Length; i++)
            {
                if (charaWork.additionalCommandAcquired[i] != false)
                    propPacketUtil.addProperty(String.Format("charaWork.additionalCommandAcquired[{0}]", i));
            }
            
            for (int i = 0; i < charaWork.parameterSave.commandSlot_compatibility.Length; i++)
            {
                charaWork.parameterSave.commandSlot_compatibility[i] = true;
                if (charaWork.parameterSave.commandSlot_compatibility[i])
                    propPacketUtil.addProperty(String.Format("charaWork.parameterSave.commandSlot_compatibility[{0}]", i));
            }

         /*
      for (int i = 0; i < charaWork.parameterSave.commandSlot_recastTime.Length; i++)
      {
          if (charaWork.parameterSave.commandSlot_recastTime[i] != 0)
              propPacketUtil.addProperty(String.Format("charaWork.parameterSave.commandSlot_recastTime[{0}]", i));
      }            
      */

            //System
            propPacketUtil.addProperty("charaWork.parameterTemp.forceControl_float_forClientSelf[0]");
            propPacketUtil.addProperty("charaWork.parameterTemp.forceControl_float_forClientSelf[1]");
            propPacketUtil.addProperty("charaWork.parameterTemp.forceControl_int16_forClientSelf[0]");
            propPacketUtil.addProperty("charaWork.parameterTemp.forceControl_int16_forClientSelf[1]");

            charaWork.parameterTemp.otherClassAbilityCount[0] = 4;
            charaWork.parameterTemp.otherClassAbilityCount[1] = 5;
            charaWork.parameterTemp.giftCount[1] = 5;

            propPacketUtil.addProperty("charaWork.parameterTemp.otherClassAbilityCount[0]");
            propPacketUtil.addProperty("charaWork.parameterTemp.otherClassAbilityCount[1]");
            propPacketUtil.addProperty("charaWork.parameterTemp.giftCount[1]");

            propPacketUtil.addProperty("charaWork.depictionJudge");
            
            //Scenario
            for (int i = 0; i < playerWork.questScenario.Length; i++)
            {
                if (playerWork.questScenario[i] != 0)
                    propPacketUtil.addProperty(String.Format("playerWork.questScenario[{0}]", i));
            }

            //Guildleve - Local
            for (int i = 0; i < playerWork.questGuildleve.Length; i++)
            {
                if (playerWork.questGuildleve[i] != 0)
                    propPacketUtil.addProperty(String.Format("playerWork.questGuildleve[{0}]", i));
            }

            //Guildleve - Regional
            for (int i = 0; i < work.guildleveId.Length; i++)
            {
                if (work.guildleveId[i] != 0)
                    propPacketUtil.addProperty(String.Format("work.guildleveId[{0}]", i));
                if (work.guildleveDone[i] != false)
                    propPacketUtil.addProperty(String.Format("work.guildleveDone[{0}]", i));
                if (work.guildleveChecked[i] != false)
                    propPacketUtil.addProperty(String.Format("work.guildleveChecked[{0}]", i));
            }

            //NPC Linkshell
            for (int i = 0; i < playerWork.npcLinkshellChatCalling.Length; i++)
            {
                if (playerWork.npcLinkshellChatCalling[i] != false)
                    propPacketUtil.addProperty(String.Format("playerWork.npcLinkshellChatCalling[{0}]", i));
                if (playerWork.npcLinkshellChatExtra[i] != false)
                    propPacketUtil.addProperty(String.Format("playerWork.npcLinkshellChatExtra[{0}]", i));
            }

            propPacketUtil.addProperty("playerWork.restBonusExpRate");

            //Profile
            propPacketUtil.addProperty("playerWork.tribe");
            propPacketUtil.addProperty("playerWork.guardian");
            propPacketUtil.addProperty("playerWork.birthdayMonth");
            propPacketUtil.addProperty("playerWork.birthdayDay");
            propPacketUtil.addProperty("playerWork.initialTown");
            
            return BasePacket.createPacket(propPacketUtil.done(), true, false);
        }

        public void sendZoneInPackets(WorldManager world, ushort spawnType)
        {
            queuePacket(SetActorIsZoningPacket.buildPacket(actorId, actorId, false));
            queuePacket(_0x10Packet.buildPacket(actorId, 0xFF));
            queuePacket(SetMusicPacket.buildPacket(actorId, zone.bgmDay, 0x01));
            queuePacket(SetWeatherPacket.buildPacket(actorId, SetWeatherPacket.WEATHER_CLEAR, 1));
            
            queuePacket(SetMapPacket.buildPacket(actorId, zone.regionId, zone.actorId));

            queuePacket(getSpawnPackets(actorId, spawnType));            
            getSpawnPackets(actorId, spawnType).debugPrintPacket();

            #region grouptest
            //Retainers
            List<ListEntry> retainerListEntries = new List<ListEntry>();
            retainerListEntries.Add(new ListEntry(actorId, 0xFFFFFFFF, 0x139E, false, true, customDisplayName));
            retainerListEntries.Add(new ListEntry(0x23, 0x0, 0xFFFFFFFF, false, false, "TEST1"));
            retainerListEntries.Add(new ListEntry(0x24, 0x0, 0xFFFFFFFF, false, false, "TEST2"));
            retainerListEntries.Add(new ListEntry(0x25, 0x0, 0xFFFFFFFF, false, false, "TEST3"));
            BasePacket retainerListPacket = BasePacket.createPacket(ListUtils.createRetainerList(actorId, 0xF4, 1, 0x800000000004e639, retainerListEntries), true, false);
            playerSession.queuePacket(retainerListPacket);

            //Party
            List<ListEntry> partyListEntries = new List<ListEntry>();
            partyListEntries.Add(new ListEntry(actorId, 0xFFFFFFFF, 0xFFFFFFFF, false, true, customDisplayName));
            partyListEntries.Add(new ListEntry(0x029B27D3, 0xFFFFFFFF, 0x195, false, true, "Valentine Bluefeather"));
            BasePacket partyListPacket = BasePacket.createPacket(ListUtils.createPartyList(actorId, 0xF4, 1, 0x8000000000696df2, partyListEntries), true, false);
            playerSession.queuePacket(partyListPacket);
            #endregion

            #region Inventory & Equipment
            queuePacket(InventoryBeginChangePacket.buildPacket(actorId));
            inventories[Inventory.NORMAL].sendFullInventory();
            inventories[Inventory.CURRENCY].sendFullInventory();
            inventories[Inventory.KEYITEMS].sendFullInventory();
            inventories[Inventory.BAZAAR].sendFullInventory();
            inventories[Inventory.MELDREQUEST].sendFullInventory();
            inventories[Inventory.LOOT].sendFullInventory();
            equipment.SendFullEquipment(false);   
            playerSession.queuePacket(InventoryEndChangePacket.buildPacket(actorId), true, false);
            #endregion

            playerSession.queuePacket(getInitPackets(actorId));


            BasePacket areaMasterSpawn = zone.getSpawnPackets(actorId);
            BasePacket debugSpawn = world.GetDebugActor().getSpawnPackets(actorId);
            BasePacket worldMasterSpawn = world.GetActor().getSpawnPackets(actorId);
            BasePacket weatherDirectorSpawn = new WeatherDirector(this, 8003).getSpawnPackets(actorId);
            BasePacket directorSpawn = null;
            
            if (currentDirector != null)
                directorSpawn = currentDirector.getSpawnPackets(actorId);

            playerSession.queuePacket(areaMasterSpawn);
            playerSession.queuePacket(debugSpawn);
            if (directorSpawn != null)
            {
                //directorSpawn.debugPrintPacket();
               // currentDirector.getInitPackets(actorId).debugPrintPacket();
                queuePacket(directorSpawn);
                queuePacket(currentDirector.getInitPackets(actorId));
                //queuePacket(currentDirector.getSetEventStatusPackets(actorId));
            }
            playerSession.queuePacket(worldMasterSpawn);

            if (zone.isInn)
            {
                SetCutsceneBookPacket cutsceneBookPacket = new SetCutsceneBookPacket();
                for (int i = 0; i < 2048; i++)
                    cutsceneBookPacket.cutsceneFlags[i] = true;

                SubPacket packet = cutsceneBookPacket.buildPacket(actorId, "<Path Companion>", 11, 1, 1);

                packet.debugPrintSubPacket();
                queuePacket(packet);
            }

            playerSession.queuePacket(weatherDirectorSpawn);

/*
            #region hardcode
            BasePacket reply10 = new BasePacket("./packets/login/login10.bin"); //Item Storage, Inn Door created
            BasePacket reply11 = new BasePacket("./packets/login/login11.bin"); //NPC Create ??? Final init
            reply10.replaceActorID(actorId);
            reply11.replaceActorID(actorId);
            //playerSession.queuePacket(reply10);
           // playerSession.queuePacket(reply11);
            #endregion
*/
        }

        private void sendRemoveInventoryPackets(List<ushort> slots)
        {
            int currentIndex = 0;

            while (true)
            {
                if (slots.Count - currentIndex >= 64)
                    queuePacket(InventoryRemoveX64Packet.buildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex >= 32)
                    queuePacket(InventoryRemoveX32Packet.buildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex >= 16)
                    queuePacket(InventoryRemoveX16Packet.buildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex >= 8)
                    queuePacket(InventoryRemoveX08Packet.buildPacket(actorId, slots, ref currentIndex));
                else if (slots.Count - currentIndex == 1)
                    queuePacket(InventoryRemoveX01Packet.buildPacket(actorId, slots[currentIndex]));
                else
                    break;
            }

        }

        public bool isMyPlayer(uint otherActorId)
        {
            return actorId == otherActorId;
        }        

        public void queuePacket(BasePacket packet)
        {
            playerSession.queuePacket(packet);
        }

        public void queuePacket(SubPacket packet)
        {
            playerSession.queuePacket(packet, true, false);
        }

        public void queuePackets(List<SubPacket> packets)
        {
            foreach (SubPacket subpacket in packets)
                playerSession.queuePacket(subpacket, true, false);
        }

        public void broadcastPacket(SubPacket packet, bool sendToSelf)
        {
            if (sendToSelf)            
                queuePacket(packet);
            
            foreach (Actor a in playerSession.actorInstanceList)
            {
                if (a is Player)
                {
                    Player p = (Player)a;
                    SubPacket clonedPacket = new SubPacket(packet, a.actorId);
                    p.queuePacket(clonedPacket);
                }
            }
        }

        public void setDCFlag(bool flag)
        {
            if (flag)
            {
                broadcastPacket(SetActorIconPacket.buildPacket(actorId, actorId, SetActorIconPacket.DISCONNECTING), true);
            }
            else
            {
                if (isGM)
                    broadcastPacket(SetActorIconPacket.buildPacket(actorId, actorId, SetActorIconPacket.ISGM), true);
                else
                    broadcastPacket(SetActorIconPacket.buildPacket(actorId, actorId, 0), true);
            }
        }

        public void cleanupAndSave()
        {                        
            //Remove actor from zone and main server list
            zone.removeActorFromZone(this);
            Server.getServer().removePlayer(this);

            //Save Player
            Database.savePlayerPlayTime(this);
            Database.savePlayerPosition(this);

            Log.info(String.Format("{0} has been logged out and saved.", this.customDisplayName));
        }

        public Area getZone()
        {
            return zone;
        }

        public void sendMessage(uint logType, string sender, string message)
        {
            queuePacket(SendMessagePacket.buildPacket(actorId, actorId, logType, sender, message));
        }

        public void logout()
        {
            queuePacket(LogoutPacket.buildPacket(actorId));
            cleanupAndSave();
        }

        public void quitGame()
        {
            queuePacket(QuitPacket.buildPacket(actorId));
            cleanupAndSave();
        }

        public uint getPlayTime(bool doUpdate)
        {
            if (doUpdate)
            {
                uint curTime = Utils.UnixTimeStampUTC();
                playTime += curTime - lastPlayTimeUpdate;
                lastPlayTimeUpdate = curTime;
            }

            return playTime;
        }

        public void changeMusic(ushort musicId)
        {
            queuePacket(SetMusicPacket.buildPacket(actorId, musicId, 1));
        }

        public void sendChocoboAppearance()
        {
            broadcastPacket(SetCurrentMountChocoboPacket.buildPacket(actorId, chocoboAppearance), true);
        }

        public void sendGoobbueAppearance()
        {
            broadcastPacket(SetCurrentMountGoobbuePacket.buildPacket(actorId, 1), true);
        }

        public void setMountState(byte mountState)
        {
            this.mountState = mountState;
        }

        public byte getMountState()
        {
            return mountState;
        }

        public void doEmote(uint emoteId)
        {
            broadcastPacket(ActorDoEmotePacket.buildPacket(actorId, actorId, currentTarget, emoteId), true);
        }

        public void sendGameMessage(Actor sourceActor, Actor textIdOwner, ushort textId, byte log, params object[] msgParams)
        {
            if (msgParams.Length == 0)
            {
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, sourceActor.actorId, textIdOwner.actorId, textId, log));
            }
            else
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, sourceActor.actorId, textIdOwner.actorId, textId, log, LuaUtils.createLuaParamList(msgParams)));
        }

        public void sendGameMessage(Actor textIdOwner, ushort textId, byte log, params object[] msgParams)
        {
            if (msgParams.Length == 0)
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, textIdOwner.actorId, textId, log));
            else
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, textIdOwner.actorId, textId, log, LuaUtils.createLuaParamList(msgParams)));
        }

        public void sendGameMessage(Actor textIdOwner, ushort textId, byte log, string customSender, params object[] msgParams)
        {
            if (msgParams.Length == 0)
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, textIdOwner.actorId, textId, customSender, log));
            else
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, textIdOwner.actorId, textId, customSender, log, LuaUtils.createLuaParamList(msgParams)));
        }

        public void sendGameMessage(Actor textIdOwner, ushort textId, byte log, uint displayId, params object[] msgParams)
        {
            if (msgParams.Length == 0)
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, textIdOwner.actorId, textId, displayId, log));
            else
                queuePacket(GameMessagePacket.buildPacket(Server.GetWorldManager().GetActor().actorId, actorId, textIdOwner.actorId, textId, displayId, log, LuaUtils.createLuaParamList(msgParams)));
        }

        public void broadcastWorldMessage(ushort worldMasterId, params object[] msgParams)
        {
            //SubPacket worldMasterMessage = 
            //zone.broadcastPacketAroundActor(this, worldMasterMessage);
        }

        public void graphicChange(uint slot, uint graphicId)
        {
            appearanceIds[slot] = graphicId;           
        }

        public void graphicChange(uint slot, uint weapId, uint equipId, uint variantId, uint colorId)
        {

            uint mixedVariantId;

            if (weapId == 0)
                mixedVariantId = ((variantId & 0x1F) << 5) | colorId;
            else
                mixedVariantId = variantId;

            uint graphicId =
                    (weapId & 0x3FF)  << 20 |
                    (equipId & 0x3FF) << 10 |
                    (mixedVariantId & 0x3FF);

            appearanceIds[slot] = graphicId;            
            
        }

        public void sendAppearance()
        {
            broadcastPacket(createAppearancePacket(actorId), true);
        }

        public void sendCharaExpInfo()
        {
            if (lastStep == 0)
            {
                int maxLength;
                if ((sizeof(short) * charaWork.battleSave.skillLevel.Length)-lastPosition < 0x5E)
                    maxLength = (sizeof(short) * charaWork.battleSave.skillLevel.Length) - lastPosition;
                else
                    maxLength = 0x5E;

                byte[] skillLevelBuffer = new byte[maxLength];
                Buffer.BlockCopy(charaWork.battleSave.skillLevel, 0, skillLevelBuffer, 0, skillLevelBuffer.Length);
                SetActorPropetyPacket charaInfo1 = new SetActorPropetyPacket("charaWork/exp");

                charaInfo1.setIsArrayMode(true);
                if (maxLength == 0x5E)
                {
                    charaInfo1.addBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevel", 0), skillLevelBuffer, 0, skillLevelBuffer.Length, 0x0);
                    lastPosition += maxLength;
                }
                else
                {
                    charaInfo1.addBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevel", 0), skillLevelBuffer, 0, skillLevelBuffer.Length, 0x3);
                    lastPosition = 0;
                    lastStep++;
                }

                charaInfo1.addTarget();

                queuePacket(charaInfo1.buildPacket(actorId, actorId));
            }
            else if (lastStep == 1)
            {
                int maxLength;
                if ((sizeof(short) * charaWork.battleSave.skillLevelCap.Length) - lastPosition < 0x5E)
                    maxLength = (sizeof(short) * charaWork.battleSave.skillLevelCap.Length) - lastPosition;
                else
                    maxLength = 0x5E;

                byte[] skillCapBuffer = new byte[maxLength];
                Buffer.BlockCopy(charaWork.battleSave.skillLevelCap, lastPosition, skillCapBuffer, 0, skillCapBuffer.Length);
                SetActorPropetyPacket charaInfo1 = new SetActorPropetyPacket("charaWork/exp");

                
                if (maxLength == 0x5E)
                {
                    charaInfo1.setIsArrayMode(true);
                    charaInfo1.addBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevelCap", 0), skillCapBuffer, 0, skillCapBuffer.Length, 0x1);
                    lastPosition += maxLength;
                }
                else
                {
                    charaInfo1.setIsArrayMode(false);
                    charaInfo1.addBuffer(Utils.MurmurHash2("charaWork.battleSave.skillLevelCap", 0), skillCapBuffer, 0, skillCapBuffer.Length, 0x3);
                    lastStep = 0;
                    lastPosition = 0;
                }

                charaInfo1.addTarget();

                queuePacket(charaInfo1.buildPacket(actorId, actorId));
            }
           
        }

        public InventoryItem[] getGearset(ushort classId)
        {
            return Database.getEquipment(this, classId);
        }

        public void prepareClassChange(byte classId)
        {            
            //If new class, init abilties and level

            sendCharaExpInfo();
        }

        public void doClassChange(byte classId)
        {
            //load hotbars
            //Calculate stats
            //Calculate hp/mp

            //Get Potenciel ??????
            
            //Set HP/MP/TP PARAMS

            //Set mainskill and level
            
            //Set Parameters

            //Set current EXP

            //Set Hotbar Commands 1
            //Set Hotbar Commands 2
            //Set Hotbar Commands 3

            //Check if bonus point available... set

            //Set rested EXP

            charaWork.parameterSave.state_mainSkill[0] = classId;
            charaWork.parameterSave.state_mainSkillLevel = charaWork.battleSave.skillLevel[classId-1];

            playerWork.restBonusExpRate = 0.0f;

            ActorPropertyPacketUtil propertyBuilder = new ActorPropertyPacketUtil("charaWork/stateForAll", this, actorId);

            propertyBuilder.addProperty("charaWork.parameterSave.state_mainSkill[0]");
            propertyBuilder.addProperty("charaWork.parameterSave.state_mainSkillLevel");
            propertyBuilder.newTarget("playerWork/expBonus");
            propertyBuilder.addProperty("playerWork.restBonusExpRate");

            List<SubPacket> packets = propertyBuilder.done();

            foreach (SubPacket packet in packets)
                broadcastPacket(packet, true);

            Database.savePlayerCurrentClass(this);
        }

        public void graphicChange(int slot, InventoryItem invItem)
        {
            if (invItem == null)            
                appearanceIds[slot] = 0;            
            else
            {
                Item item = Server.getItemGamedata(invItem.itemId);
                if (item is EquipmentItem)
                {
                    EquipmentItem eqItem = (EquipmentItem)item;

                    uint mixedVariantId;
                    
                    if (eqItem.graphicsWeaponId == 0)
                        mixedVariantId = ((eqItem.graphicsVariantId & 0x1F) << 5) | eqItem.graphicsColorId;
                    else
                        mixedVariantId = eqItem.graphicsVariantId;

                    uint graphicId =
                            (eqItem.graphicsWeaponId & 0x3FF) << 20 |
                            (eqItem.graphicsEquipmentId & 0x3FF) << 10 |
                            (mixedVariantId & 0x3FF);

                    appearanceIds[slot] = graphicId;
                }
            }

            Database.savePlayerAppearance(this);

            broadcastPacket(createAppearancePacket(actorId), true);
        }        

        public Inventory getInventory(ushort type)
        {
            if (inventories.ContainsKey(type))
                return inventories[type];
            else
                return null;
        }

        public Actor getActorInInstance(uint actorId)
        {
            foreach (Actor a in playerSession.actorInstanceList)
            {
                if (a.actorId == actorId)
                    return a;
            }

            return null;
        }

        public void setZoneChanging(bool flag)
        {
            isZoneChanging = flag;
        }

        public bool isInZoneChange()
        {
            return isZoneChanging;
        }

        public Equipment getEquipment()
        {
            return equipment;
        }     

        public byte getInitialTown()
        {
            return playerWork.initialTown;
        }

        public int getFreeQuestSlot()
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] == null)
                    return i;
            }

            return -1;
        }

        public void addQuest(uint id)
        {
            Actor actor = Server.getStaticActors((0xA0F00000 | id));
            addQuest(actor.actorName);
        }

        public void addQuest(string name)
        {
            Actor actor = Server.getStaticActors(name);

            if (actor == null)
                return;

            uint id = actor.actorId;

            int freeSlot = getFreeQuestSlot();

            if (freeSlot == -1)
                return;

            playerWork.questScenario[freeSlot] = id;
            questScenario[freeSlot] = new Quest(this, playerWork.questScenario[freeSlot], name, null, 0);
            Database.saveQuest(this, questScenario[freeSlot]);
        }

        public Quest getQuest(uint id)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorId == (0xA0F00000 | id))
                    return questScenario[i];
            }

            return null;
        }

        public Quest getQuest(string name)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorName.ToLower().Equals(name.ToLower()))
                    return questScenario[i];
            }

            return null;
        }

        public bool hasQuest(string name)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorName.ToLower().Equals(name.ToLower()))
                    return true;
            }

            return false;
        }

        public bool hasQuest(uint id)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorId == (0xA0F00000 | id))
                    return true;
            }

            return false;
        }

        public int getQuestSlot(uint id)
        {
            for (int i = 0; i < questScenario.Length; i++)
            {
                if (questScenario[i] != null && questScenario[i].actorId == (0xA0F00000 | id))
                    return i;
            }

            return -1;
        }

        public void setDirector(string directorType, bool sendPackets)
        {
            if (directorType.Equals("openingDirector"))
            {
                currentDirector = new OpeningDirector(this, 0x46080012);
            }
            else if (directorType.Equals("QuestDirectorMan0l001"))
            {
                currentDirector = new QuestDirectorMan0l001(this, 0x46080012);
            }
            else if (directorType.Equals("QuestDirectorMan0g001"))  
            {
                currentDirector = new QuestDirectorMan0g001(this, 0x46080012);
            }
            else if (directorType.Equals("QuestDirectorMan0u001"))
            {
                currentDirector = new QuestDirectorMan0u001(this, 0x46080012);
            }

            if (sendPackets)
            {
                queuePacket(RemoveActorPacket.buildPacket(actorId, 0x46080012));
                queuePacket(currentDirector.getSpawnPackets(actorId));
                queuePacket(currentDirector.getInitPackets(actorId));
                //queuePacket(currentDirector.getSetEventStatusPackets(actorId));
                //currentDirector.getSpawnPackets(actorId).debugPrintPacket();
                //currentDirector.getInitPackets(actorId).debugPrintPacket();
            }

        }

        public Director getDirector()
        {
            return currentDirector;
        }

        public void examinePlayer(Actor examinee)
        {
            Player toBeExamined;
            if (examinee is Player)
                toBeExamined = (Player)examinee;
            else
                return;

            queuePacket(InventoryBeginChangePacket.buildPacket(toBeExamined.actorId, actorId));
            toBeExamined.getEquipment().SendCheckEquipmentToPlayer(this);
            queuePacket(InventoryEndChangePacket.buildPacket(toBeExamined.actorId, actorId));
        }

        public void sendRequestedInfo(params object[] parameters)
        {
            List<LuaParam> lParams = LuaUtils.createLuaParamList(parameters);
            SubPacket spacket = InfoRequestResponsePacket.buildPacket(actorId, actorId, lParams);
            spacket.debugPrintSubPacket();
            queuePacket(spacket);
        }

        public void kickEvent(Actor actor, string conditionName, params object[] parameters)
        {
            if (actor == null)
                return;

            List<LuaParam> lParams = LuaUtils.createLuaParamList(parameters);
            SubPacket spacket = KickEventPacket.buildPacket(actorId, actor.actorId, conditionName, lParams);
            spacket.debugPrintSubPacket();
            queuePacket(spacket);
        }

        public void setEventStatus(Actor actor, string conditionName, bool enabled, byte unknown)
        {
            queuePacket(SetEventStatus.buildPacket(actorId, actor.actorId, enabled, unknown, conditionName));
        }

        public void runEventFunction(string functionName, params object[] parameters)
        {
            List<LuaParam> lParams = LuaUtils.createLuaParamList(parameters);
            SubPacket spacket = RunEventFunctionPacket.buildPacket(actorId, currentEventOwner, currentEventName, functionName, lParams);
            spacket.debugPrintSubPacket();
            queuePacket(spacket);
        }

        public void endEvent()
        {
            SubPacket p = EndEventPacket.buildPacket(actorId, currentEventOwner, currentEventName);
            p.debugPrintSubPacket();
            queuePacket(p);

            currentEventOwner = 0;
            currentEventName = "";
            eventMenuId = 0;
        }

        public void endCommand()
        {
            SubPacket p = EndEventPacket.buildPacket(actorId, currentCommand, currentCommandName);
            p.debugPrintSubPacket();
            queuePacket(p);

            currentCommand = 0;
            currentCommandName = "";
        }

        public void setCurrentMenuId(uint id)
        {
            eventMenuId = id;
        }

        public uint getCurrentMenuId()
        {
            return eventMenuId;
        }

        public void sendInstanceUpdate()
        {
           
            //Update Instance
            playerSession.updateInstance(zone.getActorsAroundActor(this, 50));            
        
        }

    }
}
