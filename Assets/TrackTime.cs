using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class TrackTime : MonoBehaviour
{

    private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

    // bool doorOpen
    // bool lightningOn
    // bool rainOn

    private string data = "";

    private Dictionary<int, float> timeLightningIntensity = new Dictionary<int, float>();

    private Dictionary<int, float> timeRainIntensity = new Dictionary<int, float>();

    private Dictionary<string, float> timeSpentOnEachIntesity = new Dictionary<string, float>();

    private int doorOpen = 0;
    private int rainToggle = 0;
    private int lightningToggle = 0;
    private int lightningItensity = 0;
    private int rainIntensity = 0;
    private int outsideLights = 0;
    private int insideLights = 0;
    private int numberOfLightning = 0;

    [SerializeField] private TemplateForSavingValuesInCSV csvSaver;

    //private int

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

        timeSpentOnEachIntesity["Intensidade Trovoada 0"] = 0f;
        timeSpentOnEachIntesity["Intensidade Trovoada 1"] = 0f;
        timeSpentOnEachIntesity["Intensidade Trovoada 2"] = 0f;
        timeSpentOnEachIntesity["Intensidade Trovoada 3"] = 0f;
        timeSpentOnEachIntesity["Intensidade Trovoada 4"] = 0f;
        timeSpentOnEachIntesity["Intensidade Trovoada 5"] = 0f;

        timeSpentOnEachIntesity["Itensidade Chuva 0"] = 0f;
        timeSpentOnEachIntesity["Itensidade Chuva 1"] = 0f;
        timeSpentOnEachIntesity["Itensidade Chuva 2"] = 0f;
        timeSpentOnEachIntesity["Itensidade Chuva 3"] = 0f;
        timeSpentOnEachIntesity["Itensidade Chuva 4"] = 0f;
        timeSpentOnEachIntesity["Itensidade Chuva 5"] = 0f;

        timeSpentOnEachIntesity["Intensidade Luzes na Rua 0"] = 0f;
        timeSpentOnEachIntesity["Intensidade Luzes na Rua 1"] = 0f;
        timeSpentOnEachIntesity["Intensidade Luzes na Rua 2"] = 0f;
        timeSpentOnEachIntesity["Intensidade Luzes na Rua 3"] = 0f;
        timeSpentOnEachIntesity["Intensidade Luzes na Rua 4"] = 0f;
        timeSpentOnEachIntesity["Intensidade Luzes na Rua 5"] = 0f;

        timeSpentOnEachIntesity["Itensidade Luzes em Casa 0"] = 0f;
        timeSpentOnEachIntesity["Itensidade Luzes em Casa 1"] = 0f;
        timeSpentOnEachIntesity["Itensidade Luzes em Casa 2"] = 0f;
        timeSpentOnEachIntesity["Itensidade Luzes em Casa 3"] = 0f;
        timeSpentOnEachIntesity["Itensidade Luzes em Casa 4"] = 0f;
        timeSpentOnEachIntesity["Itensidade Luzes em Casa 5"] = 0f;

        timeSpentOnEachIntesity["Porta Aberta"] = 0f;
        timeSpentOnEachIntesity["Porta Fechada"] = 0f;

        timeSpentOnEachIntesity["Trovoada Ligada"] = 0f;
        timeSpentOnEachIntesity["Trovoada Desligada"] = 0f;

        timeSpentOnEachIntesity["Chuva Ligada"] = 0f;
        timeSpentOnEachIntesity["Chuva Desligada"] = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("File created");
            string timeOfEachIntensity = "";

            foreach (KeyValuePair<string, float> entry in timeSpentOnEachIntesity)
            {
                string timeSpent = string.Format("{0:D2}m:{1:D2}s:{2:D3}ms\n",
                TimeSpan.FromSeconds(entry.Value).Minutes,
                TimeSpan.FromSeconds(entry.Value).Seconds,
                TimeSpan.FromSeconds(entry.Value).Milliseconds);
                timeOfEachIntensity += entry.Key + " : " + timeSpent;

                csvSaver.CreateCSV_OR_UPDATE(entry.Key, timeSpent, "175");
            }
            timeOfEachIntensity += $"Número de Relâmpagos durante a experiência : {numberOfLightning}";
            csvSaver.CreateCSV_OR_UPDATE("Número de Relâmpagos durante a experiência", numberOfLightning.ToString(), "175");
            data = timeOfEachIntensity + data;
            System.IO.File.WriteAllText(desktopPath + $"\\TrovoadaDataFile175.txt", data);
        }

        TrackTimeAndIntensity();
    }

    public void CreateFile(string participant)
    {

    }

    private void TrackTimeAndIntensity()
    {
        if(lightningToggle == 1)
        {
            timeSpentOnEachIntesity[$"Intensidade Trovoada {lightningItensity}"] += Time.deltaTime;
            timeSpentOnEachIntesity["Trovoada Ligada"] += Time.deltaTime;
        }
        else
        {
            timeSpentOnEachIntesity["Trovoada Desligada"] += Time.deltaTime;
        }

        if(rainToggle == 1)
        {
            timeSpentOnEachIntesity[$"Itensidade Chuva {rainIntensity}"] += Time.deltaTime;
            timeSpentOnEachIntesity["Chuva Ligada"] += Time.deltaTime;
        }
        else
        {
            timeSpentOnEachIntesity["Chuva Desligada"] += Time.deltaTime;
        }

        if (doorOpen == 1)
        {
            timeSpentOnEachIntesity["Porta Aberta"] += Time.deltaTime;
        }
        else
        {
            timeSpentOnEachIntesity["Porta Fechada"] += Time.deltaTime;
        }

        timeSpentOnEachIntesity[$"Intensidade Luzes na Rua {outsideLights}"] += Time.deltaTime;

        timeSpentOnEachIntesity[$"Itensidade Luzes em Casa {insideLights}"] += Time.deltaTime;

    }

    public void LightningIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Intensidade Trovoada Changed to {intensity}\n";
        lightningItensity = intensity ;
    }

    public void RainIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Itensidade Chuva Changed to {intensity}\n";
        rainIntensity = intensity ;
    }

    public void OutsideLightsIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Outside Lights Level Changed to {intensity}\n";
        outsideLights = intensity ;
    }

    public void InsideLightsIntesityChange(int intensity)
    {
        data += $"{GetCurrentTime()}: Inside Lights Level Changed to {intensity}\n";
        insideLights = intensity ;
    }

    public void ToggleLightning(bool toggle)
    {
        data += $"{GetCurrentTime()}: Lightning turned {(toggle? "on" : "off")}\n";
        lightningToggle = toggle ? 1 : 0;
    }

    public void ToggleRain(bool toggle)
    {
        data += $"{GetCurrentTime()}: Rain turned {(toggle ? "on" : "off")}\n";
        rainToggle = toggle ? 1 : 0;
    }

    public void ToggleDoor(bool toggle)
    {
        data += $"{GetCurrentTime()}: Door is now {(toggle ? "open" : "closed")}\n";
        doorOpen = toggle ? 1 : 0;
    }

    public void CreateLightning()
    {
        data += $"{GetCurrentTime()}: Lightning created\n";
        numberOfLightning++;
    }

    public void HappenedLightning()
    {
        data += $"{GetCurrentTime()}: Lightning happened\n";
        numberOfLightning++;
    }

    public string GetCurrentTime()
    {
        return string.Format("{0:D2}m:{1:D2}s:{2:D3}ms",
                TimeSpan.FromSeconds(Time.timeSinceLevelLoad).Minutes,
                TimeSpan.FromSeconds(Time.timeSinceLevelLoad).Seconds,
                TimeSpan.FromSeconds(Time.timeSinceLevelLoad).Milliseconds);
    }


}
