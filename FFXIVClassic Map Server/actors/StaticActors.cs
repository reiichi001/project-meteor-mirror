using FFXIVClassic_Map_Server.actors.judge;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.actors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.actors
{
    class StaticActors
    {
        private Dictionary<uint, Actor> mStaticActors = new Dictionary<uint, Actor>();

        public StaticActors()
        {
            //Judges
            mStaticActors.Add(0xA0F5BAF1, new Judge(0x4BAF1, "JudgeMaster"));
            mStaticActors.Add(0xA0F5E201, new Judge(0x4E201, "CommonJudge"));
            mStaticActors.Add(0xA0F5E206, new Judge(0x4E206, "TutorialJudge"));
            mStaticActors.Add(0xA0F5E20D, new Judge(0x4E20D, "ChocoboJudge"));
            mStaticActors.Add(0xA0F50911, new Judge(0x50911, "DeceptionJudge"));

            //Commands
            mStaticActors.Add(0xA0F02EE2, new Command(0xA0F02EE2, "ResetOccupiedCommand"));
            mStaticActors.Add(0xA0F02EE3, new Command(0xA0F02EE3, "CombinationManagementCommand"));
            mStaticActors.Add(0xA0F02EE4, new Command(0xA0F02EE4, "CombinationStartCommand"));
            mStaticActors.Add(0xA0F02EE5, new Command(0xA0F02EE5, "BonusPointCOmmand"));
            mStaticActors.Add(0xA0F02EE7, new Command(0xA0F02EE7, "ChangeEquipCommand"));
            mStaticActors.Add(0xA0F02EE9, new Command(0xA0F02EE9, "EquipCommand"));
            mStaticActors.Add(0xA0F02EEA, new Command(0xA0F02EEA, "EquipAbilityCommand"));
            mStaticActors.Add(0xA0F02EEB, new Command(0xA0F02EEB, "PartyTargetCommand"));
            mStaticActors.Add(0xA0F02EED, new Command(0xA0F02EED, "EquipPartsShowHideCommand"));
            mStaticActors.Add(0xA0F02EEE, new Command(0xA0F02EEE, "ChocoboRideCommand"));
            mStaticActors.Add(0xA0F02EEF, new Command(0xA0F02EEF, "ChocoboRideCommand"));
            mStaticActors.Add(0xA0F02EF0, new Command(0xA0F02EF0, "ReverseInputOperationCommand"));
            mStaticActors.Add(0xA0F02EF1, new Command(0xA0F02EF1, "ChangeJobCommand"));
            mStaticActors.Add(0xA0F05209, new Command(0xA0F05209, "ActivateCommand"));
            mStaticActors.Add(0xA0F0520A, new Command(0xA0F0520A, "ActivateCommand"));
            mStaticActors.Add(0xA0F0520D, new Command(0xA0F0520D, "CommandCancelCommand"));
            mStaticActors.Add(0xA0F0520E, new Command(0xA0F0520E, "CommandCancelCommand"));
            mStaticActors.Add(0xA0F0520F, new Command(0xA0F0520F, "ItemCommand"));
            mStaticActors.Add(0xA0F05210, new Command(0xA0F05210, "AutoAttackTargetChangeCommand"));
            mStaticActors.Add(0xA0F055FC, new Command(0xA0F055FC, "CraftCommand"));
            mStaticActors.Add(0xA0F055FD, new Command(0xA0F055FD, "CraftCommand"));
            mStaticActors.Add(0xA0F055FF, new Command(0xA0F055FF, "CraftCommand"));
            mStaticActors.Add(0xA0F05E25, new Command(0xA0F05E25, "TalkCommand"));
            mStaticActors.Add(0xA0F05E26, new Command(0xA0F05E26, "EmoteStandardCommand"));
            mStaticActors.Add(0xA0F05E28, new Command(0xA0F05E28, "ContinueCommand"));
            mStaticActors.Add(0xA0F05E29, new Command(0xA0F05E29, "LoginEventCommand"));
            mStaticActors.Add(0xA0F05E8B, new Command(0xA0F05E8B, "PartyInviteCommand"));            
            mStaticActors.Add(0xA0F05E8C, new Command(0xA0F05E8C, "PartyJoinCommand"));
            mStaticActors.Add(0xA0F05E8D, new Command(0xA0F05E8D, "PartyResignCommand"));
            mStaticActors.Add(0xA0F05E8E, new Command(0xA0F05E8E, "PartyBreakupCommand"));
            mStaticActors.Add(0xA0F05E8F, new Command(0xA0F05E8F, "PartyKickCommand"));
            mStaticActors.Add(0xA0F05E90, new Command(0xA0F05E90, "PartyLeaderCommand"));
            mStaticActors.Add(0xA0F05E91, new Command(0xA0F05E91, "PartyAcceptCommand"));
            mStaticActors.Add(0xA0F05E93, new Command(0xA0F05E93, "RequestQuestJournalCommand"));
            mStaticActors.Add(0xA0F05E94, new Command(0xA0F05E94, "RequestInformationCommand"));            
            mStaticActors.Add(0xA0F05E95, new Command(0xA0F05E95, "NpcLinkshellChatCommand"));
            mStaticActors.Add(0xA0F05E96, new Command(0xA0F05E96, "BazaarDealCommand"));
            mStaticActors.Add(0xA0F05E97, new Command(0xA0F05E97, "BazaarCheckCommand"));
            mStaticActors.Add(0xA0F05E98, new Command(0xA0F05E98, "BazaarUndealCommand"));
            mStaticActors.Add(0xA0F05E99, new Command(0xA0F05E99, "TradeOfferCommand"));
            mStaticActors.Add(0xA0F05E9A, new Command(0xA0F05E9A, "TradeExecuteCommand"));
            mStaticActors.Add(0xA0F05E9B, new Command(0xA0F05E9B, "LogoutCommand"));
            mStaticActors.Add(0xA0F05E9C, new Command(0xA0F05E9C, "TeleportCommand"));
            mStaticActors.Add(0xA0F05E9D, new Command(0xA0F05E9D, "ItemStuffCommand"));
            mStaticActors.Add(0xA0F05E9E, new Command(0xA0F05E9E, "ItemArrangementCommand"));
            mStaticActors.Add(0xA0F05E9F, new Command(0xA0F05E9F, "ItemMovePackageCommand"));
            mStaticActors.Add(0xA0F05EA0, new Command(0xA0F05EA0, "ItemSplitCommand"));
            mStaticActors.Add(0xA0F05EA1, new Command(0xA0F05EA1, "ItemTransferCommand"));
            mStaticActors.Add(0xA0F05EA2, new Command(0xA0F05EA2, "ItemWasteCommand"));
            mStaticActors.Add(0xA0F05EA3, new Command(0xA0F05EA3, "BazaarTradeCommand"));
            mStaticActors.Add(0xA0F05EA4, new Command(0xA0F05EA4, "WidgetOpenCommand"));
            mStaticActors.Add(0xA0F05EA5, new Command(0xA0F05EA5, "MacroCommand"));
            mStaticActors.Add(0xA0F05EA6, new Command(0xA0F05EA6, "TradeOfferCancelCommand"));
            mStaticActors.Add(0xA0F05EA7, new Command(0xA0F05EA7, "LinkshellAppointCommand"));
            mStaticActors.Add(0xA0F05EA8, new Command(0xA0F05EA8, "LinkshellInviteCommand"));
            mStaticActors.Add(0xA0F05EA9, new Command(0xA0F05EA9, "LinkshellInviteCancelCommand"));
            mStaticActors.Add(0xA0F05EAA, new Command(0xA0F05EAA, "LinkshellKickCommand"));
            mStaticActors.Add(0xA0F05EAB, new Command(0xA0F05EAB, "LinkshellResignCommand"));
            mStaticActors.Add(0xA0F05EAC, new Command(0xA0F05EAC, "LinkshellChangeCommand"));
            mStaticActors.Add(0xA0F05EAE, new Command(0xA0F05EAE, "CheckCommand"));
            mStaticActors.Add(0xA0F05EAF, new Command(0xA0F05EAF, "NetStatUserSwitchCommand"));
            mStaticActors.Add(0xA0F05EB0, new Command(0xA0F05EB0, "ItemMaterializeCommand"));
            mStaticActors.Add(0xA0F05EB1, new Command(0xA0F05EB1, "JournalCommand"));
            mStaticActors.Add(0xA0F05EB2, new Command(0xA0F05EB2, "DiceCommand"));
            mStaticActors.Add(0xA0F05EB3, new Command(0xA0F05EB3, "RepairOrderCommand"));
            mStaticActors.Add(0xA0F05EB4, new Command(0xA0F05EB4, "RepairEquipmentsCommand"));
            mStaticActors.Add(0xA0F05EED, new Command(0xA0F05EED, "PlaceDrivenCommand"));
            mStaticActors.Add(0xA0F05EEE, new Command(0xA0F05EEE, "ContentCommand"));            
            mStaticActors.Add(0xA0F05EEF, new Command(0xA0F05EEF, "ConfirmGroupCommand"));
            mStaticActors.Add(0xA0F05EF0, new Command(0xA0F05EF0, "ConfirmWarpCOmmand"));
            mStaticActors.Add(0xA0F05EF1, new Command(0xA0F05EF1, "ConfirmTradeCommand"));
            mStaticActors.Add(0xA0F05EF2, new Command(0xA0F05EF2, "ConfirmRaiseCommand"));            
            mStaticActors.Add(0xA0F05EF8, new Command(0xA0F05EF8, "EmoteSitCommand "));
            mStaticActors.Add(0xA0F06A0E, new Command(0xA0F06A0E, "AttackWeaponSKill"));
            mStaticActors.Add(0xA0F07339, new Command(0xA0F07339, "NegotiateCommand"));
            mStaticActors.Add(0xA0F07595, new Command(0xA0F07595, "DebugInputCommand"));
        }

        public bool exists(uint actorId)
        {
            return mStaticActors[actorId] != null;
        }

        public Actor getActor(uint actorId)
        {
            return mStaticActors[actorId];
        }

    }
    
}
