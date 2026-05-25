using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Analytics;

public class CardDisplay : MonoBehaviour
{
    public TMP_Text displayText;
    public FirestoreInicialize cardAccess;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cardAccess = GameObject.FindGameObjectWithTag("BD").GetComponent<FirestoreInicialize>();
    }

    public void OnTargetFound(Transform imageTargetTransform)
    {
        string cardName = imageTargetTransform.name;
        Debug.Log($"Carta encontrada: {cardName}");

        displayText = imageTargetTransform.Find("Text").GetComponent<TextMeshPro>();
        if (displayText != null) 
        {
            //Actividad 2 recuperar datos desde firestore
            cardAccess.FetchCardDataFromFirestore(cardName, displayText);
        }
        else 
        {
            Debug.LogError("Objeto de texto no encontrado");
        }
    }

    public void OnTargetLost(Transform imageTargetTransform) 
     {
        displayText = imageTargetTransform.Find("Text").GetComponent<TextMeshPro>();
        if (displayText != null) 
        {
            displayText.text = "Buscando carta...";
        }
        else
        {
            Debug.LogError("Objeto de texto no encontrado");
        }
    }
}
