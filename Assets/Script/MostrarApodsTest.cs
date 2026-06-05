using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class MostrarApodsTest : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropdown;
    [SerializeField] private Renderer quadRenderer;

    private FirebaseFirestore firestore;

    private Dictionary<string, APODInfo> apods =
        new Dictionary<string, APODInfo>();

    private void Start()
    {
        firestore = FirebaseFirestore.DefaultInstance;
    }

    public void CargarLista()
    {
        dropdown.ClearOptions();
        apods.Clear();

        // Opción inicial
        dropdown.options.Add(
            new TMP_Dropdown.OptionData("Elegir imagen...")
        );

        firestore.Collection("APOD_Imagenes")
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted) return;

                foreach (DocumentSnapshot doc in task.Result.Documents)
                {
                    APODInfo info = new APODInfo();

                    info.titulo = doc.GetValue<string>("Titulo");
                    info.url = doc.GetValue<string>("HDUrl");

                    apods[info.titulo] = info;

                    dropdown.options.Add(
                        new TMP_Dropdown.OptionData(info.titulo)
                    );
                }

                dropdown.RefreshShownValue();
            });
    }

    public void MostrarSeleccionado()
    {
        string titulo =
            dropdown.options[dropdown.value].text;

        APODInfo info = apods[titulo];

        StartCoroutine(CargarImagen(info.url));
    }

    IEnumerator CargarImagen(string url)
    {
        Debug.Log(url);

        UnityWebRequest request =
            UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D tex =
                DownloadHandlerTexture.GetContent(request);

            //rawImage.texture = tex;
        }
        else
        {
            Debug.LogError(request.error);
        }
    }

    [System.Serializable]
    public class APODInfo
    {
        public string titulo;
        public string url;
    }
}
