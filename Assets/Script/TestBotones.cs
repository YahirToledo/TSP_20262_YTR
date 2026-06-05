using Firebase.Extensions;
using Firebase.Firestore;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestBotones : MonoBehaviour
{
    [Header("Firestore")]
    private FirebaseFirestore firestore;

    [Header("UI")]
    [SerializeField] private RawImage rawImage;

    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button buttonPrefab;

    [SerializeField] private Button siguienteBtn;
    [SerializeField] private Button anteriorBtn;

    private List<APODInfo> apods = new List<APODInfo>();

    private int paginaActual = 0;
    private const int elementosPorPagina = 5;

    private void Start()
    {
        firestore = FirebaseFirestore.DefaultInstance;

        siguienteBtn.onClick.AddListener(PaginaSiguiente);
        anteriorBtn.onClick.AddListener(PaginaAnterior);

        //CargarLista();
    }

    public void CargarLista()
    {
        apods.Clear();

        firestore.Collection("APOD_Imagenes")
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (!task.IsCompleted)
                {
                    Debug.LogError("Error obteniendo documentos");
                    return;
                }

                foreach (DocumentSnapshot doc in task.Result.Documents)
                {
                    APODInfo info = new APODInfo();

                    info.titulo = doc.GetValue<string>("Titulo");
                    info.url = doc.GetValue<string>("HDUrl");

                    apods.Add(info);
                }

                paginaActual = 0;
                MostrarPagina();
            });
    }

    public void MostrarPagina()
    {
        // Borrar botones anteriores
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        int inicio = paginaActual * elementosPorPagina;
        int fin = Mathf.Min(inicio + elementosPorPagina, apods.Count);

        for (int i = inicio; i < fin; i++)
        {
            APODInfo info = apods[i];

            Button nuevoBoton =
                Instantiate(buttonPrefab, contentPanel);

            TMP_Text textoBoton =
                nuevoBoton.GetComponentInChildren<TMP_Text>();

            textoBoton.text = info.titulo;

            string url = info.url;

            nuevoBoton.onClick.AddListener(() =>
            {
                StartCoroutine(CargarImagen(url));
            });

            //nuevoBoton.onClick.RemoveAllListeners();

            //string urlLocal = info.url;

            //nuevoBoton.onClick.AddListener(() =>
            //{
            //    StartCoroutine(CargarImagen(urlLocal));
            //});
        }

        anteriorBtn.interactable = paginaActual > 0;

        siguienteBtn.interactable =
            fin < apods.Count;
    }

    public void PaginaSiguiente()
    {
        if ((paginaActual + 1) * elementosPorPagina < apods.Count)
        {
            paginaActual++;
            MostrarPagina();
        }
    }

    public void PaginaAnterior()
    {
        if (paginaActual > 0)
        {
            paginaActual--;
            MostrarPagina();
        }
    }

    IEnumerator CargarImagen(string url)
    {
        Debug.Log("Cargando: " + url);

        UnityWebRequest request =
            UnityWebRequestTexture.GetTexture(url);

        yield return request.SendWebRequest();

        if (request.result ==
            UnityWebRequest.Result.Success)
        {
            Texture2D tex =
                DownloadHandlerTexture.GetContent(request);

            rawImage.texture = tex;
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
