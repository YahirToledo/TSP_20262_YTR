using Firebase.Database;
using Firebase.Extensions;
using Firebase.Firestore;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    [SerializeField]
    [TextArea]
    private string apiKey;

    private static FirebaseFirestore firestore;

    [SerializeField]
    private TMP_InputField imageDateInput;

    private void Awake()
    {
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public void FetchAndStoreImageData()
    {
        string imageDate = imageDateInput.text;
        StartCoroutine(GetApodData(imageDate));
    }

    private IEnumerator GetApodData(string imageDate)
    {
        string url;

        if (string.IsNullOrWhiteSpace(imageDate))
        {
            url = $"https://api.nasa.gov/planetary/apod?api_key={UnityWebRequest.EscapeURL(apiKey)}";
        }
        else
        {
            url = $"https://api.nasa.gov/planetary/apod?date={imageDate}&api_key={UnityWebRequest.EscapeURL(apiKey)}";
        }

        Debug.Log(url);

        UnityWebRequest request = UnityWebRequest.Get(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            ProcessImageData(json);
        }
        else
        {
            Debug.LogError("Error obteniendo datos: " + request.error);
        }
    }

    public void ProcessImageData(string json)
    {
        var apod = JsonUtility.FromJson<APODData>(json);

        // Solo permitir imágenes
        if (apod.media_type != "image")
        {
            Debug.Log("La APOD seleccionada es un video. No se guardará en Firestore.");
            return;
        }

        string imageTitle = apod.title.Replace("/", "-");

        DocumentReference docRef = firestore
            .Collection("APOD_Imagenes")
            .Document(imageTitle);

        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"Fecha", apod.date},
            {"Explicacion", apod.explanation},
            {"Titulo", apod.title},
            {"Tipo", apod.media_type},
            {"HDUrl", apod.hdurl}
        };

        docRef.SetAsync(data).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Imagen '{imageTitle}' almacenada correctamente.");
            }
            else
            {
                Debug.LogError("Error guardando datos: " + task.Exception);
            }
        });
    }

    [System.Serializable]
    public class APODData
    {
        public string date;
        public string explanation;
        public string hdurl;
        public string media_type;
        public string title;
        public string url;
    }

}
