using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.common
{
    namespace EfficientHashTables
    {
        public class Efficient64bitHashTable<T>
        {
            private class element
            {
                public ulong _key;
                public T _value;
            };
            private element[][] _buckets;
            private uint _capacity;

            public Efficient64bitHashTable()
            {
                _capacity = 214373;    // some prime number
                _buckets = new element[_capacity][];
            }
            public Efficient64bitHashTable(uint capacity)
            {
                _capacity = capacity;
                _buckets = new element[_capacity][];
            }

            public uint hash(ulong key)
            {
                return (uint)(key % _capacity);
            }

            public void Add(ulong key, T value)
            {
                uint hsh = hash(key);
                element[] e;
                if (_buckets[hsh] == null)
                    _buckets[hsh] = e = new element[1];
                else
                {
                    foreach (var elem in _buckets[hsh])
                        if (elem._key == key)
                        {
                            elem._value = value;
                            return;
                        }
                    e = new element[_buckets[hsh].Length + 1];
                    Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
                    _buckets[hsh] = e;
                }
                e[0] = new element { _key = key, _value = value };
            }

            public T Get(ulong key)
            {
                uint hsh = hash(key);
                element[] e = _buckets[hsh];
                if (e == null) return default(T);
                foreach (var f in e)
                    if (f._key == key)
                        return f._value;
                return default(T);
            }

            public bool Has(ulong key)
            {
                uint hsh = hash(key);
                element[] e = _buckets[hsh];
                if (e == null) return false;
                foreach (var f in e)
                    if (f._key == key)
                        return true;
                return false;
            }

            public int Count()
            {
                int r = 0;
                foreach (var e in _buckets)
                    if (e != null)
                        r += e.Length;
                return r;
            }
        }

        public class Efficient32bitHashTable<T>
        {
            private class element
            {
                public uint _key;
                public T _value;
            };
            private element[][] _buckets;
            private uint _capacity;

            public Efficient32bitHashTable()
            {
                _capacity = 463;    // some prime number
                _buckets = new element[_capacity][];
            }
            public Efficient32bitHashTable(uint capacity)
            {
                _capacity = capacity;
                _buckets = new element[_capacity][];
            }

            public uint hash(uint key)
            {
                return (uint)(key % _capacity);
            }

            public void Add(uint key, T value)
            {
                uint hsh = hash(key);
                element[] e;
                if (_buckets[hsh] == null)
                    _buckets[hsh] = e = new element[1];
                else
                {
                    foreach (var elem in _buckets[hsh])
                        if (elem._key == key)
                        {
                            elem._value = value;
                            return;
                        }
                    e = new element[_buckets[hsh].Length + 1];
                    Array.Copy(_buckets[hsh], 0, e, 1, _buckets[hsh].Length);
                    _buckets[hsh] = e;
                }
                e[0] = new element { _key = key, _value = value };
            }         

            public T Get(uint key)
            {
                uint hsh = hash(key);
                element[] e = _buckets[hsh];
                if (e == null) return default(T);
                foreach (var f in e)
                    if (f._key == key)
                        return f._value;
                return default(T);
            }

            public int Count()
            {
                int r = 0;
                foreach (var e in _buckets)
                    if (e != null)
                        r += e.Length;
                return r;
            }
            


        }
    }

}
