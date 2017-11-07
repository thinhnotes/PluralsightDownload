﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace THttpWebRequest
{
    internal class RandomHelper
    {
        private static readonly Random r = new Random();

        public static string GetRandomStringFrom(IEnumerable<string> s, int maxLengh)
        {
            int n = s.Count();
            string[] arr = s.ToArray();
            var sb = new StringBuilder();

            while (sb.ToString().Length < maxLengh)
            {
                sb.Append(arr[r.Next(n - 1)]);
            }
            return sb.ToString();
        }

        public static string GetRandomStringFromWithLoop(IEnumerable<string> s, int loopTimes = 1)
        {
            int n = s.Count();
            string[] arr = s.ToArray();
            var sb = new StringBuilder();
            for (int i = 0; i < loopTimes; i++)
            {
                sb.Append(arr[r.Next(n - 1)]);
            }

            return sb.ToString();
        }


        public static string GetRandomStringFrom(string s, int maxLengh)
        {
            return GetRandomStringFrom(s.ToCharArray().Select(c => c.ToString()), maxLengh);
        }
    }
}