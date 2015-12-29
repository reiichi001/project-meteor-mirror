using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.list
{
    class ListUtils
    {

        public static List<SubPacket> createList(uint actorId, uint locationCode, ulong sequenceId, ulong listId, uint listTypeId, List<ListEntry> listEntries)
        {
            List<SubPacket> subpacketList = new List<SubPacket>();
            subpacketList.Add(ListStartPacket.buildPacket(actorId, locationCode, sequenceId, listId, listTypeId, listEntries.Count));
            subpacketList.Add(ListBeginPacket.buildPacket(actorId, locationCode, sequenceId, listId, listEntries.Count));
            subpacketList.Add(ListEntriesEndPacket.buildPacket(actorId, locationCode, sequenceId, listEntries, 0));
            subpacketList.Add(ListEndPacket.buildPacket(actorId, locationCode, sequenceId, listId));
            return subpacketList;
        }

        public static List<SubPacket> createRetainerList(uint actorId, uint locationCode, ulong sequenceId, ulong listId, List<ListEntry> listEntries)
        {
            List<SubPacket> subpacketList = new List<SubPacket>();
            subpacketList.Add(ListStartPacket.buildPacket(actorId, locationCode, sequenceId, listId, ListStartPacket.TYPEID_RETAINER, listEntries.Count));
            subpacketList.Add(ListBeginPacket.buildPacket(actorId, locationCode, sequenceId, listId, listEntries.Count));
            subpacketList.Add(ListEntriesEndPacket.buildPacket(actorId, locationCode, sequenceId, listEntries, 0));
            subpacketList.Add(ListEndPacket.buildPacket(actorId, locationCode, sequenceId, listId));
            return subpacketList;
        }

        public static List<SubPacket> createPartyList(uint actorId, uint locationCode, ulong sequenceId, ulong listId, List<ListEntry> listEntries)
        {
            List<SubPacket> subpacketList = new List<SubPacket>();
            subpacketList.Add(ListStartPacket.buildPacket(actorId, locationCode, sequenceId, listId, ListStartPacket.TYPEID_PARTY, listEntries.Count));
            subpacketList.Add(ListBeginPacket.buildPacket(actorId, locationCode, sequenceId, listId, listEntries.Count));
            subpacketList.Add(ListEntriesEndPacket.buildPacket(actorId, locationCode, sequenceId, listEntries, 0));
            subpacketList.Add(ListEndPacket.buildPacket(actorId, locationCode, sequenceId, listId));
            return subpacketList;
        }

    }
}
