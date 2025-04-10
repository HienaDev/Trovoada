using System;
using System.IO;
using System.Text;
using UnityEngine;

public class TemplateForSavingValuesInCSV : MonoBehaviour
{
    //Path where the file is saved, example:  path = Application.dataPath + "GameFiles.csv";
    private string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);


    private StreamWriter writer; 


    // Creates a new csv file or updates the older one
    public void CreateCSV_OR_UPDATE(string category, string timeSpent, string numberOfParticipant)
    {
        Debug.Log($"Try to save + {category} and {timeSpent}");
        if (!File.Exists(desktopPath + $"\\TrovoadaDataFile{numberOfParticipant}.csv"))
        {
            // Specify UTF-8 encoding when creating the file
            using (writer = new StreamWriter(desktopPath + $"\\TrovoadaDataFile{numberOfParticipant}.csv", false, Encoding.UTF8))
            {
                // adds headers if the file is new
                writer.WriteLine(CreateHeaders());
                // fill the next line with the values
                writer.Write($"{category}," + $"{timeSpent}");
            }
        } else
        {
            // Specify UTF-8 encoding when creating the file
            using (writer = new StreamWriter(desktopPath + $"\\TrovoadaDataFile{numberOfParticipant}.csv", true, Encoding.UTF8))
            {
                
                // fill the next line with the values
                writer.Write($"{category}," + $"{timeSpent}");
            }

        }
    }

    string CreateHeaders()
    {
        StringBuilder headers = new StringBuilder();
        headers.Append("Categoria e Itensidade,");
        headers.Append("Tempo");


        return headers.ToString();
    }

}
