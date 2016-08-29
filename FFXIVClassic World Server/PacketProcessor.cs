using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using FFXIVClassic_World_Server.Packets.Receive;
using FFXIVClassic_World_Server.Packets.Send;
using FFXIVClassic_World_Server.Packets.Send.Login;
using System;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_World_Server
{
    class PacketProcessor
    {
        /*
        Session Creation:

            Get 0x1 from server
            Send 0x7
            Send 0x2

        Zone Change:

            Send 0x7
            Get 0x8 - Wait??
            Send 0x2
        */


        Server mServer;

        public PacketProcessor(Server server)
        {           
            mServer = server;
        }     

        public void ProcessPacket(ClientConnection client, BasePacket packet)
        {                      
            if (packet.header.isCompressed == 0x01)                       
                BasePacket.DecompressPacket(ref packet);

            List<SubPacket> subPackets = packet.GetSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {
                subpacket.DebugPrintSubPacket();

                //Initial Connect Packet, Create session
                if (subpacket.header.type == 0x01)
                {                    
                    HelloPacket hello = new HelloPacket(packet.data);

                    if (packet.header.connectionType == BasePacket.TYPE_ZONE)
                        mServer.AddSession(client, Session.Channel.ZONE, hello.sessionId);
                    else if (packet.header.connectionType == BasePacket.TYPE_CHAT)
                        mServer.AddSession(client, Session.Channel.CHAT, hello.sessionId);

                    client.QueuePacket(_0x7Packet.BuildPacket(0x0E016EE5), true, false);
                    client.QueuePacket(_0x2Packet.BuildPacket(hello.sessionId), true, false);
                }
                //Ping from World Server
                else if (subpacket.header.type == 0x07)
                {
                    SubPacket init = _0x8PingPacket.BuildPacket(client.owner.sessionId);
                    client.QueuePacket(BasePacket.CreatePacket(init, true, false));
                }
                //Zoning Related
                else if (subpacket.header.type == 0x08)
                {
                    //Response, client's current [actorID][time]
                    //BasePacket init = Login0x7ResponsePacket.BuildPacket(BitConverter.ToUInt32(packet.data, 0x10), Utils.UnixTimeStampUTC(), 0x07);
                    //client.QueuePacket(init);
                    packet.DebugPrintPacket();
                }
                //Game Message
                else if (subpacket.header.type == 0x03)
                {
                    //Send to the correct zone server
                    uint targetSession = subpacket.header.targetId;
                    mServer.GetSession(targetSession).routing1 = Server.GetServer().GetWorldManager().mZoneServerList["127.0.0.1:1989"];

                    if (mServer.GetSession(targetSession).routing1 != null)
                        mServer.GetSession(targetSession).routing1.SendPacket(subpacket);

                    if (mServer.GetSession(targetSession).routing2 != null)
                        mServer.GetSession(targetSession).routing2.SendPacket(subpacket);
                }
                else
                    packet.DebugPrintPacket();
            }
        }    

    }
}
