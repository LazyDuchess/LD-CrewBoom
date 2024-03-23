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
    private const string RegistryValueCopyBundles = "CopyBundles";
    private const string RegistryValueTargetBundlePath = "TargetBundlePath";
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

    public static bool CopyBundles
    {
        get
        {
            return Registry.GetValue(RegistryKey, RegistryValueCopyBundles, "false") as string == "true" ? true : false;
        }
        set
        {
            Registry.SetValue(RegistryKey, RegistryValueCopyBundles, value == true ? "true" : "false");
        }
    }

    public static string TargetBundlePath
    {
        get
        {
            return Registry.GetValue(RegistryKey, RegistryValueTargetBundlePath, "") as string;
        }
        set
        {
            Registry.SetValue(RegistryKey, RegistryValueTargetBundlePath, value);
        }
    }
}
