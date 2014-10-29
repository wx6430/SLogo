using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SLogo.Helper
{
    /// <summary>
    /// 标准错误输出流
    /// </summary>
    public static class Stderr
    {
        private static TextWriter err = Console.Error;

        public static void WriteLine()
        {
            err.WriteLine();
        }

        public static void WriteLine(bool value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(char value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(char[] buffer)
        {
            err.WriteLine(buffer);
        }

        public static void WriteLine(decimal value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(double value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(float value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(int value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(long value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(object value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(uint value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(ulong value)
        {
            err.WriteLine(value);
        }

        public static void WriteLine(string format, object arg0)
        {
            err.WriteLine(format, arg0);
        }

        public static void WriteLine(string format, params object[] arg)
        {
            err.WriteLine(format, arg);
        }

        public static void WriteLine(char[] buffer, int index, int count)
        {
            err.WriteLine(buffer, index, count);
        }

        public static void WriteLine(string format, object arg0, object arg1)
        {
            err.WriteLine(format, arg0, arg1);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2)
        {
            err.WriteLine(format, arg0, arg1, arg2);
        }

        public static void WriteLine(string format, object arg0, object arg1, object arg2, object arg3)
        {
            err.WriteLine(format, arg0, arg1, arg2, arg3);
        }
    }
}
