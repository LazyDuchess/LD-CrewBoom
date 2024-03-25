using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;

public static class Preferences
{
    private const string RegistryKey = @"HKEY_CURRENT_USER\Software\LDCrewBoom\CrewBoom";
    private const string RegistryValueAuthorName = "AuthorName";
    private const string RegistryValueCopyBundles = "CopyBundles";
    private const string RegistryValueTargetBundlePath = "TargetBundlePath";
    private const string RegistryValueOpenFileExplorerOnBuild = "OpenFileExplorerOnBuild";
    private const string RegistryValueAutoUpdate = "AutoUpdate";

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
            return Registry.GetValue(RegistryKey, RegistryValueCopyBundles, "false") as string == "true";
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

    public static bool OpenFileExplorerOnBuild
    {
        get
        {
            return Registry.GetValue(RegistryKey, RegistryValueOpenFileExplorerOnBuild, "true") as string == "true";
        }
        set
        {
            Registry.SetValue(RegistryKey, RegistryValueOpenFileExplorerOnBuild, value == true ? "true" : "false");
        }
    }

    public static bool AutoUpdate
    {
        get
        {
            return Registry.GetValue(RegistryKey, RegistryValueAutoUpdate, "true") as string == "true";
        }
        set
        {
            Registry.SetValue(RegistryKey, RegistryValueAutoUpdate, value == true ? "true" : "false");
        }
    }

    // Might implement at a later point. Don't wanna make the preferences too confusing.
    public static bool AllowEditPrefabInstances
    {
        get
        {
            return true;
        }
    }
}
