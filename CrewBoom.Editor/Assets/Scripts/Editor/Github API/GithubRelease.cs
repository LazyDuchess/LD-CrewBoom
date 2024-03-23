using System;

[Serializable]
public class GithubRelease
{
    public string tag_name;
    public GithubAsset[] assets;
}