using FFXIVClassic.Common;
using System;
using System.IO;

using System.Collections.Generic;

namespace  FFXIVClassic_Map_Server.packets.send.actor.battle
{
    class BattleActionX18Packet
    {
        public const ushort OPCODE = 0x013B;
        public const uint PACKET_SIZE = 0x148;

        public static SubPacket BuildPacket(uint sourceActorId, uint animationId, ushort commandId, BattleAction[] actionList, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (actionList.Length - listOffset <= 18)
                        max = actionList.Length - listOffset;
                    else
                        max = 18;

                    binWriter.Write((UInt32)sourceActorId);
                    binWriter.Write((UInt32)animationId);

                    //Missing... last value is float, string in here as well?

                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt32)max); //Num actions
                    binWriter.Write((UInt16)commandId);
                    binWriter.Write((UInt16)0x818); //?

                    binWriter.Seek(0x28, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt32)actionList[listOffset + i].targetId);
                    
                    binWriter.Seek(0x70, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].amount);

                    binWriter.Seek(0x94, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].worldMasterTextId);

                    binWriter.Seek(0xB8, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt32)actionList[listOffset + i].effectId);

                    binWriter.Seek(0x100, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte)actionList[listOffset + i].param);

                    binWriter.Seek(0x112, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte)actionList[listOffset + i].hitNum);

                    listOffset += max;
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint animationId, ushort commandId, List<BattleAction> actionList, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (actionList.Count - listOffset <= 18)
                        max = actionList.Count - listOffset;
                    else
                        max = 18;

                    binWriter.Write((UInt32)sourceActorId);
                    binWriter.Write((UInt32)animationId);

                    //Missing... last value is float, string in here as well?

                    binWriter.Seek(0x20, SeekOrigin.Begin);
                    binWriter.Write((UInt32)max); //Num actions
                    binWriter.Write((UInt16)commandId);
                    binWriter.Write((UInt16)0x818); //?

                    binWriter.Seek(0x28, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt32)actionList[listOffset + i].targetId);

                    binWriter.Seek(0x70, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].amount);

                    binWriter.Seek(0x94, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt16)actionList[listOffset + i].worldMasterTextId);

                    binWriter.Seek(0xB8, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((UInt32)actionList[listOffset + i].effectId);

                    binWriter.Seek(0x100, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte)actionList[listOffset + i].param);

                    binWriter.Seek(0x112, SeekOrigin.Begin);
                    for (int i = 0; i < max; i++)
                        binWriter.Write((Byte)actionList[listOffset + i].hitNum);

                    listOffset += max;
                }
            }

            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
