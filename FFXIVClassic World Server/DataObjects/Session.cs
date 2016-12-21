using FFXIVClassic_World_Server.Packets.Send.Subpackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_World_Server.DataObjects
{
    class Session
    {
        public enum Channel {ZONE, CHAT};

        public readonly uint sessionId;

        public string characterName;
        public uint currentZoneId;

        public readonly ClientConnection clientConnection;
        public readonly Channel type;
        public ZoneServer routing1, routing2;

        public Session(uint sessionId, ClientConnection connection, Channel type)
        {
            this.sessionId = sessionId;
            this.clientConnection = connection;
            this.type = type;
            connection.owner = this;
            Database.LoadZoneSessionInfo(this);
        }
       
        public void SendGameMessage( ushort textId, byte log, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, sessionId, 0x5FF80001, textId, log), true, false);
            else
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, sessionId, 0x5FF80001, textId, log, LuaUtils.CreateLuaParamList(msgParams)), true, false);
        }

        public void SendGameMessage( ushort textId, byte log, string customSender, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, sessionId, 0x5FF80001, textId, customSender, log), true, false);
            else
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, sessionId, 0x5FF80001, textId, customSender, log, LuaUtils.CreateLuaParamList(msgParams)), true, false);
        }

        public void SendGameMessage(ushort textId, byte log, uint displayId, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, sessionId, 0x5FF80001, textId, displayId, log), true, false);
            else
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, sessionId, 0x5FF80001, textId, displayId, log, LuaUtils.CreateLuaParamList(msgParams)), true, false);
        }

    }
}
