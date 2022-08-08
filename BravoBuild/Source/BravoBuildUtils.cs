using System;
using System.IO;
using UnityEngine;

public static class BravoBuildUtils
{
    private static Color VerifyFileExists(string path)
    {
        return File.Exists(path) ? Color.green : Color.red;
    }
}