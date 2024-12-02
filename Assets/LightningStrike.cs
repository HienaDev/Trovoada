using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class LightningStrike : MonoBehaviour
{

    [SerializeField] private Light lightiningStrike;

    private Vector2 delay = new Vector2(0.3f, 3f);
    private float currentDelay;
    [SerializeField] private Vector2[] delayMeter;

    [SerializeField] private Vector2 timeLightStaysOn = new Vector2(0.1f, 0.3f);
    private float currentTime;

    [SerializeField] private Vector2 lightIntensity = new Vector2(10f, 100f);
    [SerializeField] private Vector2 lightSound = new Vector2(0.3f, 1f);

    [SerializeField] private Vector2 nextStrikeCooldown = new Vector2(0.3f, 3f);
    private float strikeCooldown;
    private bool lightningReady;
    private float timer = 0f;

    private AudioSource lightingSound;

    [SerializeField] private AudioClip weakLightning;
    [SerializeField] private AudioClip mediumLightning;
    [SerializeField] private AudioClip strongLightning;
    private List<AudioClip> lightningSounds = new List<AudioClip>();

    [SerializeField] private AudioSource rainSound;

    private bool lightning = true;

    [SerializeField] private Slider lightingIntensitySlider;
    private int[] sliderGrid = new int[6] { 0, 20, 40, 60, 80, 100 };


    private int currentDelayForDebug;

    private IEnumerator Start()
    {

        currentDelayForDebug = (int)(lightingIntensitySlider.value * 5f);

        ChangeLightningIntensity();

        lightningReady = true;
        lightningSounds.Add(weakLightning);
        lightningSounds.Add(mediumLightning);
        lightningSounds.Add(strongLightning);

        //lightiningStrike = GetComponent<Light>();
        lightingSound = GetComponent<AudioSource>();

        currentDelay = Random.Range(delay.x, delay.y);
        currentTime = Random.Range(timeLightStaysOn.x, timeLightStaysOn.y);
        strikeCooldown = Random.Range(nextStrikeCooldown.x, nextStrikeCooldown.y);

        yield return new WaitForSeconds(2f);

        //StartCoroutine(Strike());
    }

    public void Update()
    {

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R))
        {
            ToggleRain();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.T))
        {
            ToggleLightning();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Y))
        {
            TriggerLightning();
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            if (currentDelayForDebug < 6)
            {
                currentDelayForDebug++;
                ChangeLightingWithKeys(currentDelayForDebug);
            }
        }

        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            if (currentDelayForDebug > 0)
            {
                currentDelayForDebug--;
                ChangeLightingWithKeys(currentDelayForDebug);
            }
        }



        if (lightning)
        {
            timer += Time.deltaTime;
        }

        //Debug.Log((timer) + " : " + strikeCooldown);


        if (timer >= strikeCooldown && lightningReady)
        {
            lightningReady = false;

            StartCoroutine(Strike());
        }
    }

    private IEnumerator Strike()
    {
        Debug.Log("currentDelay: " + currentDelay);
        float nextDelay = currentDelay - currentTime;

        lightiningStrike.intensity = MapRange(currentDelay, delay.x, delay.y, lightIntensity.y, lightIntensity.x);
        if (nextDelay < 0f)
        {
            PlayLightningSound();
        }

        float time = 0f;

        float flickerDelay = 0.1f;

        while (time < currentTime)
        {
            time += Time.deltaTime;
            if (time > flickerDelay)
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

        currentDelay = Random.Range(delay.x, delay.y);
        currentTime = Random.Range(timeLightStaysOn.x, timeLightStaysOn.y);
        strikeCooldown = Random.Range(nextStrikeCooldown.x, nextStrikeCooldown.y);

        lightningReady = true;
        timer = 0f;

    }

    private void PlayLightningSound()
    {
        lightingSound.volume = MapRange(currentDelay, delayMeter[5].x, delayMeter[0].y, lightSound.y, lightSound.x);
        lightingSound.pitch = Random.Range(0.9f, 1.1f);
        lightingSound.clip = lightningSounds[Random.Range(0, lightningSounds.Count)];
        lightingSound.Play();
    }

    private float MapRange(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        return toMin + (value - fromMin) * (toMax - toMin) / (fromMax - fromMin);
    }

    public void ToggleLightning()
    {
        lightning = !lightning;
    }

    public void ToggleRain()
    {
        rainSound.enabled = !rainSound.enabled;
    }

    public void TriggerLightning()
    {
        StartCoroutine(Strike());
    }

    public void ChangeLightingWithKeys(int value)
    {
        lightingIntensitySlider.value = value * 0.2f;
    }

    public void ChangeLightningIntensity()
    {
        float currValue = lightingIntensitySlider.value * 100;

        float minDistance = float.MaxValue;
        int smallest = 0;

        for (int i = 0; i < sliderGrid.Length; i++)
        {
            if (Mathf.Abs(currValue - sliderGrid[i]) < minDistance)
            {
                minDistance = Mathf.Abs(currValue - sliderGrid[i]);
                smallest = i;
            }
        }

        switch (smallest)
        {
            case 0:
                delay = delayMeter[0];
                break;
            case 1:
                delay = delayMeter[1];
                break;
            case 2:
                delay = delayMeter[2];
                break;
            case 3:
                delay = delayMeter[3];
                break;
            case 4:
                delay = delayMeter[4];
                break;
            case 5:
                delay = delayMeter[5];
                break;
            default:
                delay = delayMeter[0];
                break;
        }

        rainSound.volume = smallest * 0.2f + 0.1f;

        Debug.Log("current lighting intesity: " + smallest);

        lightingIntensitySlider.value = smallest * 0.2f;
    }
}
