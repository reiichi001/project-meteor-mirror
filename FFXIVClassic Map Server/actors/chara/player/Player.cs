using FFXIVClassic_Lobby_Server;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.Actor.inventory;
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

namespace FFXIVClassic_Map_Server.dataobjects.chara
{
    class Player : Character
    {

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

        public uint[] timers = new uint[20];

        public ushort currentJob;

        public uint currentTitle;

        public byte gcCurrent;
        public byte gcRankLimsa;
        public byte gcRankGridania;
        public byte gcRankUldah;

        public bool hasChocobo;
        public bool hasGoobbue;
        public byte chocoboAppearance;
        public string chocoboName;

        public uint achievementPoints;

        public PlayerWork playerWork = new PlayerWork();

        public ConnectedPlayer playerSession;

        public Player(ConnectedPlayer cp, uint actorID) : base(actorID)
        {
            playerSession = cp;
            actorName = String.Format("_pc{0:00000000}", actorID);
            className = "Player";
            currentSubState = SetActorStatePacket.SUB_STATE_PLAYER;

            charaWork.property[0] = 1;
            charaWork.property[1] = 1;
            charaWork.property[2] = 1;
            charaWork.property[4] = 1;

            charaWork.command[0] =  0xA0F00000 | 21001;
            charaWork.command[1] =  0xA0F00000 | 21002;
            charaWork.command[2] =  0xA0F00000 | 12003;
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

            charaWork.command[32] = 0xA0F00000 | 27155;
            //charaWork.command[33] = 0xA0F00000 | 27150;
            charaWork.command[34] = 0xA0F00000 | 27300;

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
            
            charaWork.commandBorder = 0x20;
            
            Database.loadPlayerCharacter(this);
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
                lParams = LuaUtils.createLuaParamList("/Chara/Player/Player_work", false, false, false, true, 0, false, timers, true);
            }
            else
                lParams = LuaUtils.createLuaParamList("/Chara/Player/Player_work", false, false, false, false, false, true);
            return ActorInstantiatePacket.buildPacket(actorId, playerActorId, actorName, className, lParams);
        }        

        public override BasePacket getSpawnPackets(uint playerActorId, uint spawnType)
        {
            List<SubPacket> subpackets = new List<SubPacket>();
            subpackets.Add(createAddActorPacket(playerActorId));
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

                /*
                if (isInn)
                {
                    SetCutsceneBookPacket book = new SetCutsceneBookPacket();
                    for (int i = 0; i < book.cutsceneFlags.Length; i++)
                        book.cutsceneFlags[i] = true;
                    client.queuePacket(book.buildPacket(player.actorID), true, false); 
                 
                    //
                    //subpackets.Add(SetPlayerDreamPacket.buildPacket(playerActorId, );
                }
                 */
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
         
            /*
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
            */

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
            
            return propPacketUtil.done();
        }

        public void sendZoneInPackets(WorldManager world, ushort spawnType)
        {            
            playerSession.queuePacket(SetMapPacket.buildPacket(actorId, zone.regionId, zone.actorId), true, false);            
            playerSession.queuePacket(SetMusicPacket.buildPacket(actorId, zone.bgmDay, 0x01), true, false);
            playerSession.queuePacket(SetWeatherPacket.buildPacket(actorId, SetWeatherPacket.WEATHER_CLEAR), true, false);

            playerSession.queuePacket(getSpawnPackets(actorId, spawnType));

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

            #region itemsetup
            ////////ITEMS////////
            playerSession.queuePacket(InventoryBeginChangePacket.buildPacket(actorId), true, false);


            //TEST
            List<Item> items = new List<Item>();
            items.Add(new Item(1337, 8030920, 5)); //Leather Jacket
            items.Add(new Item(1338, 8013626, 1)); //Chocobo Mask
            items.Add(new Item(1339, 5030402, 2)); //Thyrus
            items.Add(new Item(1340, 8013635, 3)); //Dalamud Horn
            items.Add(new Item(1341, 10100132, 4)); //Savage Might 4
            items.Add(new Item(1342, 8032407, 6)); //Green Summer Halter (Female)
            items.Add(new Item(1343, 8051307, 7)); //Green Summer Tanga (Female)
            items.Add(new Item(1344, 8050766, 8)); //Flame Private's Saroul

            int count = 0;

            items[2].isHighQuality = true;
            items[0].durability = 9999;
            items[0].spiritbind = 10000;
            items[0].materia1 = 6;
            items[0].materia2 = 7;
            items[0].materia3 = 8;
            items[0].materia4 = 9;
            items[0].materia5 = 10;
            items[1].durability = 9999;
            items[2].durability = 0xFFFFFFF;
            items[3].durability = 9999;
            items[4].quantity = 99;

            //Reused
            SubPacket endInventory = InventorySetEndPacket.buildPacket(actorId);
            SubPacket beginInventory = InventorySetBeginPacket.buildPacket(actorId, 200, 00);
            SubPacket setInventory = InventoryItemPacket.buildPacket(actorId, items, ref count);

            List<SubPacket> setinvPackets = new List<SubPacket>();
            setinvPackets.Add(beginInventory);
            setinvPackets.Add(setInventory);
            setinvPackets.Add(endInventory);

            //client.queuePacket(currancy);
            //client.queuePacket(keyitems);

            #endregion
            #region equipsetup
            playerSession.queuePacket(BasePacket.createPacket(setinvPackets, true, false));
            EquipmentSetupPacket initialEqupmentPacket = new EquipmentSetupPacket();
            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_BODY, 5);
            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_HEAD, 3);
            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_UNDERSHIRT, 6);
            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_UNDERGARMENT, 7);
            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_MAINHAND, 2);
            initialEqupmentPacket.setItem(EquipmentSetupPacket.SLOT_LEGS, 8);

            //Equip Init
            playerSession.queuePacket(InventorySetBeginPacket.buildPacket(actorId, 0x23, InventorySetBeginPacket.CODE_EQUIPMENT), true, false);
            playerSession.queuePacket(BasePacket.createPacket(initialEqupmentPacket.buildPackets(actorId), true, false));
            playerSession.queuePacket(InventorySetEndPacket.buildPacket(actorId), true, false);

            playerSession.queuePacket(InventoryEndChangePacket.buildPacket(actorId), true, false);
            ////////ITEMS//////// 

            #endregion

            playerSession.queuePacket(getInitPackets(actorId));

            BasePacket innSpawn = zone.getSpawnPackets(actorId);
            BasePacket debugSpawn = world.GetDebugActor().getSpawnPackets(actorId);
            BasePacket worldMasterSpawn = world.GetActor().getSpawnPackets(actorId);
            playerSession.queuePacket(innSpawn);
            playerSession.queuePacket(debugSpawn);
            playerSession.queuePacket(worldMasterSpawn);

            #region hardcode
            BasePacket reply9 = new BasePacket("./packets/login/login9_zonesetup.bin"); //Bed, Book created
            BasePacket reply10 = new BasePacket("./packets/login/login10.bin"); //Item Storage, Inn Door created
            BasePacket reply11 = new BasePacket("./packets/login/login11.bin"); //NPC Create ??? Final init
            reply9.replaceActorID(actorId);
            reply10.replaceActorID(actorId);
            reply11.replaceActorID(actorId);
            playerSession.queuePacket(reply9);
            playerSession.queuePacket(reply10);
            playerSession.queuePacket(reply11);
            #endregion
            
        }

        public bool isMyPlayer(uint otherActorId)
        {
            return actorId == otherActorId;
        }

        

    }
}
