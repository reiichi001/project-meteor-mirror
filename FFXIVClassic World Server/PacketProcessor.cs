using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using FFXIVClassic_World_Server.Packets.Receive;
using FFXIVClassic_World_Server.Packets.Send;
using FFXIVClassic_World_Server.Packets.Send.Login;
using FFXIVClassic_World_Server.Packets.WorldPackets.Receive;
using FFXIVClassic_World_Server.Packets.WorldPackets.Send;
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
                //Initial Connect Packet, Create session
                if (subpacket.header.type == 0x01)
                {                    
                    HelloPacket hello = new HelloPacket(packet.data);

                    if (packet.header.connectionType == BasePacket.TYPE_ZONE)
                    {
                        mServer.AddSession(client, Session.Channel.ZONE, hello.sessionId);
                        mServer.GetWorldManager().DoLogin(mServer.GetSession(hello.sessionId));
                    }
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

                    if (mServer.GetSession(targetSession).routing1 != null)
                        mServer.GetSession(targetSession).routing1.SendPacket(subpacket);

                    if (mServer.GetSession(targetSession).routing2 != null)
                        mServer.GetSession(targetSession).routing2.SendPacket(subpacket);
                }
                //World Server Type
                else if (subpacket.header.type >= 0x1000)
                {
                    uint targetSession = subpacket.header.targetId;
                    Session session = mServer.GetSession(targetSession);

                    switch (subpacket.header.type)
                    {
                        //Session Begin Confirm
                        case 0x1000:
                            SessionBeginConfirmPacket beginConfirmPacket = new SessionBeginConfirmPacket(packet.data);

                            if (beginConfirmPacket.invalidPacket || beginConfirmPacket.errorCode == 0)                            
                                Program.Log.Error("Session {0} had a error beginning session.", beginConfirmPacket.sessionId);                            

                            break;
                        //Session End Confirm
                        case 0x1001:
                            SessionEndConfirmPacket endConfirmPacket = new SessionEndConfirmPacket(packet.data);
                            
                            if (!endConfirmPacket.invalidPacket && endConfirmPacket.errorCode != 0)
                            {
                                //Check destination, if != 0, update route and start new session
                                if (endConfirmPacket.destinationZone != 0)
                                {
                                    session.currentZoneId = endConfirmPacket.destinationZone;
                                    session.routing1 = Server.GetServer().GetWorldManager().GetZoneServer(endConfirmPacket.destinationZone);
                                    session.routing1.SendSessionStart(session);
                                }
                                else
                                {                                    
                                    mServer.RemoveSession(Session.Channel.ZONE, endConfirmPacket.sessionId);
                                    mServer.RemoveSession(Session.Channel.CHAT, endConfirmPacket.sessionId);
                                }
                            }
                            else
                                Program.Log.Error("Session {0} had an error ending session.", endConfirmPacket.sessionId);

                            break;                        
                        //Zone Change Request
                        case 0x1002:
                            WorldRequestZoneChangePacket zoneChangePacket = new WorldRequestZoneChangePacket(packet.data);

                            if (!zoneChangePacket.invalidPacket)
                            {
                                mServer.GetWorldManager().DoZoneServerChange(session, zoneChangePacket.destinationZoneId, "", zoneChangePacket.destinationSpawnType, zoneChangePacket.destinationX, zoneChangePacket.destinationY, zoneChangePacket.destinationZ, zoneChangePacket.destinationRot);
                            }
                           
                            break;
                    }

                }
                else
                    packet.DebugPrintPacket();
            }
        }    

    }
}
