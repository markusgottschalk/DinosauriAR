using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A plant as aquivalent for the detected plane. Many plants will be shown for a more realistic experience.
/// </summary>
public class Plant : MonoBehaviour
{
    private Vector3 localScale;

    private void Awake()
    {
        localScale = transform.localScale;
    }

    /// <summary>
    /// Slowly grow the plant when instantiated.
    /// </summary>
    /// <param name="seconds">The duration for which the plant needs to grow fully</param>
    /// <returns></returns>
    public IEnumerator GrowPlant(float seconds)
    {
        float elapsedTime = 0;
        while (elapsedTime < seconds)
        {
            gameObject.transform.localScale = Vector3.Lerp(Vector3.zero, localScale, (elapsedTime / seconds));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        gameObject.transform.localScale = localScale;
        yield return null;
    }
}
