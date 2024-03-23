using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Preferences
{
    private const string RegistryKey = @"HKEY_CURRENT_USER\Software\LDCrewBoom\CrewBoom";
    private const string RegistryValueAuthorName = "AuthorName";
    public static string AuthorName
    {
        get
        {
            return Registry.GetValue(RegistryKey, RegistryValueAuthorName, "") as string;
        }
        set
        {
            Registry.SetValue(RegistryKey, RegistryValueAuthorName, value);
        }
    }
}
