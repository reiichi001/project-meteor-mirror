using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.lua;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.events
{
    class RunEventFunctionPacket
    {
        public const ushort OPCODE = 0x0130;
        public const uint PACKET_SIZE = 0x2B8;

        public static SubPacket buildPacket(uint playerActorID, uint eventOwnerActorID, string eventStarter, string callFunction, List<LuaParam> luaParams)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            int maxBodySize = data.Length - 0x80;

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt32)playerActorID);
                    binWriter.Write((UInt32)eventOwnerActorID);
                    binWriter.Write((Byte)0);
                    binWriter.Write(Encoding.ASCII.GetBytes(eventStarter), 0, Encoding.ASCII.GetByteCount(eventStarter) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(eventStarter));
                    binWriter.Seek(0x29, SeekOrigin.Begin);
                    binWriter.Write(Encoding.ASCII.GetBytes(callFunction), 0, Encoding.ASCII.GetByteCount(callFunction) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(callFunction));
                    binWriter.Seek(0x49, SeekOrigin.Begin);

                    //Write out params
                    foreach (LuaParam p in luaParams)
                    {
                        binWriter.Write((Byte)p.typeID);
                        switch (p.typeID)
                        {
                            case 0x0: //Int32
                                binWriter.Write(Utils.swapEndian((UInt32)p.value));
                                break;
                            case 0x1: //Int32
                                binWriter.Write(Utils.swapEndian((UInt32)p.value));
                                break;
                            case 0x2: //Null Termed String                        
                                string svalue = (string)p.value;
                                binWriter.Write(Encoding.ASCII.GetBytes(svalue), 0, Encoding.ASCII.GetByteCount(svalue));
                                if (svalue[svalue.Length - 1] != '\0')
                                    binWriter.Write((Byte)0);
                                break;
                            case 0x3: //Boolean False                                
                                break;
                            case 0x4: //Boolean True                                
                                break;
                            case 0x5: //Nil                                
                                break;
                            case 0x6: //Actor (By Id)
                                binWriter.Write(Utils.swapEndian((UInt32)p.value));                                
                                break;
                            case 0x10: //Byte?
                                //value = reader.ReadByte();
                                break;
                            case 0x1B: //Short?
                                //value = reader.ReadUInt16();
                                break;                            
                        }
                    }

                    binWriter.Write((Byte)0xF);
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}
