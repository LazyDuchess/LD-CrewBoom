using UnityEngine;

public static class Extensions
{
    public static Transform FindRecursive(this Transform transform, string name)
    {
        if (transform == null) return null;

        if (transform.name == name)
        {
            return transform;
        }

        Transform next = null;
        foreach (Transform child in transform)
        {
            next = child.FindRecursive(name);
            if (next)
            {
                break;
            }
        }
        return next;
    }
}