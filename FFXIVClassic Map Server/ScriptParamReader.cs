using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server
{
    class ScriptParamReader
    {
        List<int> types = new List<int>();
        List<Object> values = new List<Object>();

        public ScriptParamReader(BinaryReader reader)
        {
            while (true)
            {
                byte code = reader.ReadByte();
                switch (code)
                {
                    case 0x0: //INT32
                        types.Add(6);
                        values.Add(reader.ReadUInt32());
                        break;
                    case 0x1: //????
                        continue;                                      
                    case 0x2: //Null Termed String
                        types.Add(2);
                        List<byte> list = new List<byte>();
                        while(true){
                            byte readByte = reader.ReadByte();
                            if (readByte == 0)
                                break;
                            list.Add(readByte);
                        }
                        values.Add(Encoding.ASCII.GetString(list.ToArray()));
                        break;
                    case 0x4: //BYTE
                        types.Add(4);
                        values.Add(reader.ReadByte());
                        break;
                    case 0x5: //NULL???
                        types.Add(5);
                        values.Add(new Object());
                        continue;  
                    case 0x6: //INT32
                        types.Add(6);
                        values.Add(reader.ReadUInt32());
                        break;
                    case 0xF: //End
                        return;
                }
            }
        }

        public int getType(int index)
        {
            return types[index];
        }

        public Object getValue(int index)
        {
            return values[index];
        }

        public int getCount()
        {
            return values.Count;
        }
    }
}
