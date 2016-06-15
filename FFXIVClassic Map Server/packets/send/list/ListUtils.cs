using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.packets.send.list
{
    class ListUtils
    {

        public static List<SubPacket> CreateList(uint actorId, uint locationCode, ulong sequenceId, ulong listId, uint listTypeId, List<ListEntry> listEntries)
        {
            List<SubPacket> subpacketList = new List<SubPacket>();
            subpacketList.Add(ListStartPacket.BuildPacket(actorId, locationCode, sequenceId, listId, listTypeId, listEntries.Count));
            subpacketList.Add(ListBeginPacket.BuildPacket(actorId, locationCode, sequenceId, listId, listEntries.Count));
            subpacketList.Add(ListEntriesEndPacket.BuildPacket(actorId, locationCode, sequenceId, listEntries, 0));
            subpacketList.Add(ListEndPacket.BuildPacket(actorId, locationCode, sequenceId, listId));
            return subpacketList;
        }

        public static List<SubPacket> CreateRetainerList(uint actorId, uint locationCode, ulong sequenceId, ulong listId, List<ListEntry> listEntries)
        {
            List<SubPacket> subpacketList = new List<SubPacket>();
            subpacketList.Add(ListStartPacket.BuildPacket(actorId, locationCode, sequenceId, listId, ListStartPacket.TYPEID_RETAINER, listEntries.Count));
            subpacketList.Add(ListBeginPacket.BuildPacket(actorId, locationCode, sequenceId, listId, listEntries.Count));
            subpacketList.Add(ListEntriesEndPacket.BuildPacket(actorId, locationCode, sequenceId, listEntries, 0));
            subpacketList.Add(ListEndPacket.BuildPacket(actorId, locationCode, sequenceId, listId));
            return subpacketList;
        }

        public static List<SubPacket> CreatePartyList(uint actorId, uint locationCode, ulong sequenceId, ulong listId, List<ListEntry> listEntries)
        {
            List<SubPacket> subpacketList = new List<SubPacket>();
            subpacketList.Add(ListStartPacket.BuildPacket(actorId, locationCode, sequenceId, listId, ListStartPacket.TYPEID_PARTY, listEntries.Count));
            subpacketList.Add(ListBeginPacket.BuildPacket(actorId, locationCode, sequenceId, listId, listEntries.Count));
            subpacketList.Add(ListEntriesEndPacket.BuildPacket(actorId, locationCode, sequenceId, listEntries, 0));
            subpacketList.Add(ListEndPacket.BuildPacket(actorId, locationCode, sequenceId, listId));
            return subpacketList;
        }

    }
}
