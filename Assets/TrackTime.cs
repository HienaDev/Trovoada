using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class TrackTime : MonoBehaviour
{

    private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    // bool doorOpen
    // bool lightningOn
    // bool rainOn

    private string data = "";

    private Dictionary<int, float> timeLightningIntensity = new Dictionary<int, float>();

    private Dictionary<int, float> timeRainIntensity = new Dictionary<int, float>();

    // Start is called before the first frame update
    void Start()
    {
        timeLightningIntensity[0] = 0;
        timeLightningIntensity[1] = 0;
        timeLightningIntensity[2] = 0;
        timeLightningIntensity[3] = 0;
        timeLightningIntensity[4] = 0;
        timeLightningIntensity[5] = 0;

        timeRainIntensity[0] = 0;
        timeRainIntensity[1] = 0;
        timeRainIntensity[2] = 0;
        timeRainIntensity[3] = 0;
        timeRainIntensity[4] = 0;
        timeRainIntensity[5] = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("File created");
            System.IO.File.WriteAllText(desktopPath + $"\\TrovoadaDataFile.txt", data);
        }
    }

    public void LightningIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Lightning Intensity Changed to {intensity}\n";
    }

    public void RainIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Rain Intensity Changed to {intensity}\n";
    }

    public void OutsideLightsIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Outside Lights Level Changed to {intensity}\n";
    }

    public void InsideLightsIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Inside Lights Level Changed to {intensity}\n";
    }

    public void ToggleLightning(bool toggle)
    {
        data += $"{GetCurrentTime()}: Lightning turned {(toggle? "on" : "off")}\n";
    }

    public void ToggleRain(bool toggle)
    {
        data += $"{GetCurrentTime()}: Rain turned {(toggle ? "on" : "off")}\n";
    }

    public void ToggleDoor(bool toggle)
    {
        data += $"{GetCurrentTime()}: Door is now {(toggle ? "open" : "closed")}\n";
    }

    public void CreateLightning()
    {
        data += $"{GetCurrentTime()}: Lightning created\n";
    }

    public void HappenedLightning()
    {
        data += $"{GetCurrentTime()}: Lightning happened\n";
    }

    public string GetCurrentTime()
    {
        return string.Format("{0:D2}m:{1:D2}s:{2:D3}ms",
                TimeSpan.FromSeconds(Time.timeSinceLevelLoad).Minutes,
                TimeSpan.FromSeconds(Time.timeSinceLevelLoad).Seconds,
                TimeSpan.FromSeconds(Time.timeSinceLevelLoad).Milliseconds);
    }


}
