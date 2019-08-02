using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperClass
{
    /// <summary>
    /// Find a child by name. Recursive breadth-first search
    /// </summary>
    /// <param name="aParent">The parent</param>
    /// <param name="aName">The name to search for</param>
    public static Transform FindDeepChild(this Transform aParent, string aName)
    {
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(aParent);
        while (queue.Count > 0)
        {
            var c = queue.Dequeue();
            if (c.name == aName)
                return c;
            foreach (Transform t in c)
                queue.Enqueue(t);
        }
        return null;
    }
}
