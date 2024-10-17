using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class LightningStrike : MonoBehaviour
{
    private Light lightiningStrike;

    [SerializeField] private Vector2 delay = new Vector2(0.3f, 3f);
    private float currentDelay;

    [SerializeField] private Vector2 timeLightStaysOn = new Vector2(0.1f, 0.3f);
    private float currentTime;

    [SerializeField] private Vector2 lightIntensity = new Vector2(10f, 100f);
    [SerializeField] private Vector2 lightSound = new Vector2(0.3f, 1f);

    [SerializeField] private Vector2 nextStrikeCooldown = new Vector2(0.3f, 3f);
    private float strikeCooldown;

    private AudioSource lightingSound;

    [SerializeField] private AudioClip weakLightning;
    [SerializeField] private AudioClip mediumLightning;
    [SerializeField] private AudioClip strongLightning;
    private List<AudioClip> lightningSounds = new List<AudioClip> ();   

    private IEnumerator Start()
    {

        lightningSounds.Add(weakLightning);
        lightningSounds.Add(mediumLightning);
        lightningSounds.Add(strongLightning);

        lightiningStrike = GetComponent<Light>();
        lightingSound = GetComponent<AudioSource>();

        currentDelay = Random.Range(delay.x, delay.y);
        currentTime = Random.Range(timeLightStaysOn.x, timeLightStaysOn.y);
        strikeCooldown = Random.Range(nextStrikeCooldown.x, nextStrikeCooldown.y);

        yield return new WaitForSeconds(2f);

        StartCoroutine(Strike());
    }

    private IEnumerator Strike()
    {
        Debug.Log(currentDelay);
        float nextDelay = currentDelay - currentTime;

        lightiningStrike.intensity = MapRange(currentDelay, delay.x, delay.y, lightIntensity.y, lightIntensity.x);
        if(nextDelay < 0f)
        {
            PlayLightningSound();
        }

        float time = 0f;

        float flickerDelay = 0.1f;

        while (time < currentTime)
        {
            time += Time.deltaTime;
            if(time > flickerDelay)
            {
                lightiningStrike.intensity += Random.Range(-10f, 10f);
                flickerDelay += 0.1f;
            }
            yield return null;
        }

        yield return new WaitForSeconds(currentTime);
        lightiningStrike.intensity = 0f;

        yield return new WaitForSeconds(nextDelay);

        if (nextDelay > 0f)
        {
            PlayLightningSound();
        }

        Debug.Log("trigger sound");

        yield return new WaitForSeconds(strikeCooldown);

        strikeCooldown = Random.Range(nextStrikeCooldown.x, nextStrikeCooldown.y);

        StartCoroutine(Strike());
    }

    private void PlayLightningSound()
    {
        lightingSound.volume = MapRange(currentDelay, delay.x, delay.y, lightSound.y, lightSound.x);
        lightingSound.pitch = Random.Range(0.9f, 1.1f);
        lightingSound.clip = lightningSounds[Random.Range(0, lightningSounds.Count)];
        lightingSound.Play();
    }

    private float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }

}
