using System;
using System.IO;
using UnityEngine;

namespace BravoBuild
{
    public static class BravoBuildUtils
    {
        private static Color VerifyFileExists(string path)
        {
            return File.Exists(path) ? Color.green : Color.red;
        }

        //TODO Remove this to from this class
        public static string ToFriendlyString(Enum code)
        {
            return Enum.GetName(code.GetType(), code);
        }
    }

}