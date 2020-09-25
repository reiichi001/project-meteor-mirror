/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using Meteor.World.Packets.Send.Subpackets;

namespace Meteor.World.DataObjects
{
    class Session
    {
        public enum Channel {ZONE, CHAT};

        public readonly uint sessionId;

        public string characterName;
        public uint currentZoneId;
        public string activeLinkshellName = "";

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

        public void SendGameMessage(uint actorId, ushort textId, byte log, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
            {
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, actorId, 0x5FF80001, textId, log));
            }
            else
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, actorId, 0x5FF80001, textId, log, LuaUtils.CreateLuaParamList(msgParams)));
        }
       
        public void SendGameMessage( ushort textId, byte log, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, 0x5FF80001, textId, log));
            else
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, 0x5FF80001, textId, log, LuaUtils.CreateLuaParamList(msgParams)));
        }

        public void SendGameMessage( ushort textId, byte log, string customSender, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, 0x5FF80001, textId, customSender, log));
            else
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, 0x5FF80001, textId, customSender, log, LuaUtils.CreateLuaParamList(msgParams)));
        }

        public void SendGameMessage(ushort textId, byte log, uint displayId, params object[] msgParams)
        {
            if (msgParams == null || msgParams.Length == 0)
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, 0x5FF80001, textId, displayId, log));
            else
                clientConnection.QueuePacket(GameMessagePacket.BuildPacket(0x5FF80001, 0x5FF80001, textId, displayId, log, LuaUtils.CreateLuaParamList(msgParams)));
        }


        public bool SetActiveLS(string name)
        {
            if (Database.SetActiveLS(this, name))
            {
                activeLinkshellName = name;
                return true;
            }
            return false;
        }
    }
}
