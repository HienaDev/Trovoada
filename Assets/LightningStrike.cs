using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightningStrike : MonoBehaviour
{
    [SerializeField] private Vector2 intensityStrike;
    private float intensityNextStrike;
    [SerializeField] private Vector2 timeToLastStrike;
    private float timeToNextStrike;
    [SerializeField] private Vector2 nextStrikeCooldown;
    private float strikeCooldown;
    private float justStruck;

    private Light lightStrike;

    private bool readyForNextStrike = true;

    // Start is called before the first frame update
    void Start()
    {
        lightStrike = GetComponent<Light>();

        intensityNextStrike = Random.Range(intensityStrike.x, intensityStrike.y);
        timeToNextStrike = Random.Range(timeToLastStrike.x, timeToLastStrike.y);
        strikeCooldown = Random.Range(nextStrikeCooldown.x, nextStrikeCooldown.y);

        StartCoroutine(Strike());
    }

    // Update is called once per frame
    void Update()
    {
        //if(readyForNextStrike && Time.time - justStruck > strikeCooldown)
        //{
        //    readyForNextStrike = false;
        //    StartCoroutine(Strike());
        //}
    }

    private IEnumerator Strike()
    {
        float lerpValue = 0f;

        while(lerpValue < 1f)
        {
            lerpValue += Time.deltaTime / (timeToNextStrike / 2);
            lightStrike.intensity = Mathf.Lerp(0, intensityNextStrike, lerpValue);
            yield return null;
        }

        while (lerpValue > 0f)
        {
            lerpValue -= Time.deltaTime / (timeToNextStrike / 2);
            lightStrike.intensity = Mathf.Lerp(0, intensityNextStrike, lerpValue);
            yield return null;
        }

        intensityNextStrike = Random.Range(intensityStrike.x, intensityStrike.y);
        timeToNextStrike = Random.Range(timeToLastStrike.x, timeToLastStrike.y);
        strikeCooldown = Random.Range(nextStrikeCooldown.x, nextStrikeCooldown.y);
        justStruck = Time.time;
        readyForNextStrike = true;
    }
}
