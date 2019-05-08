using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWebApp.Database.Models
{
    public enum Test
    {
        A,
        B,
        C
    }

    public class Blog
    {
        public int Id { get; set; }

        public bool Bool { get; set; }
        //public bool[] Bools { get; set; }
        public byte Byte { get; set; }
        public byte[] Bytes { get; set; }
        public sbyte Sbyte { get; set; }
        //public sbyte[] Sbytes { get; set; }
        public char Char { get; set; }
        //public char[] Chars { get; set; }
        public decimal Decimal { get; set; }
        //public decimal[] Decimals { get; set; }
        public double Double { get; set; }
        //public double[] Doubles { get; set; }
        public float Float { get; set; }
        //public float[] Floats { get; set; }
        public int Int { get; set; }
        //public int[] Ints { get; set; }
        public uint UInt { get; set; }
        //public uint[] UInts { get; set; }
        public long Long { get; set; }
        //public long[] Longs { get; set; }
        public ulong ULong { get; set; }
        //public ulong[] ULongs { get; set; }
        //public object Object { get; set; }
        //public object[] Objects { get; set; }
        public short Short { get; set; }
        //public short[] Shorts { get; set; }
        public ushort UShort { get; set; }
        //public ushort[] UShorts { get; set; }
        public string String { get; set; }
        public string[] Strings { get; set; }

        public Test TestEnum { get; set; }

    }

    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int BlogId { get; set; }
    }


    public class TestView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Enabled { get; set; }
    }

}