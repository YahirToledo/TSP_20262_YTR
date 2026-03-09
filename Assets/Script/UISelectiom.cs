//Necesarias
using UnityEngine;
using UnityEngine.UI; //para interfaz de usuario
using UnityEngine.Events; //par leer mov de camara
//Opcionales
using System.Collections;
using System.Collections.Generic;
using System;



public class UISelection : MonoBehaviour {
    //se definideron publicos porque...
    public static bool gazedAt; //volteaste a ver el boton si o no
    [SerializeField] //liea inmediata despues de esto, lo considera com el campo de definicion, lo sige viendo el inspector, pero ahora es privado; campo serializado
    public float fillTime = 5f; //tiempo necesario para que carge; si se cambia en el inspector, prioriza el del inspector
    public Image radialImage; //imagen que va a estar modificando
    public UnityEvent onFillComplete; //que occure cuando termina la carga, principio solid, evento, no se modifica esto, solo se extiende el codigo
                                      //onFillComplete evento generico que se genera al terminar una carga

    //Proceso Asincrono
    private Coroutine fillCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        gazedAt = false; //no iniciamos viendo el boton
        radialImage.fillAmount = 0; //el boton no tiene relleno
    }

    public void OnPointerEnter() {
        gazedAt = true; //estamos viendo el boton

        //tema de practica CONCURRENCIA
        //que la compu no se quede congelada en una sola carga, ejmplo, durante carga que no se quede pegada la camara, sino que se pueda mover y se rompa la carga
        //corrutinas: solo existen dentro de unity, sncrono, hilos, paralelo, Repasar
        if (fillCoroutine != null) {
            StopCoroutine(fillCoroutine); //si hay una rutina iniciada la detiene
        }
        fillCoroutine = StartCoroutine(fillRadial()); //si no hay corrutina la crea

        //abre o cierran la rutina,

    }

    public void OnPointerExit() {
        gazedAt = false;

        if (fillCoroutine != null) {
            StopCoroutine(fillCoroutine); //Detiene el llenado
            fillCoroutine = null;
        }
        radialImage.fillAmount = 0f; //reinicia el llenado a cero
    }


    private IEnumerator fillRadial() //cuenta el tiempo
    {
        float elapasedTime = 0f;

        while (elapasedTime < fillTime) //rango de tiempo que contamos
        {
            if (!gazedAt) //dejamos de ver el boton
            {
                yield break;
            }

            elapasedTime += Time.deltaTime; //que tanscurra el tiempo, time, reloj que permite acceder al reloj de la computadora, diferncia de tiempo
            // Acumulativo, cuanto lleva trnascurio desde que inicio hasta que lo mandaste llamar
            radialImage.fillAmount = Mathf.Clamp01(elapasedTime / fillTime);
            //relacion tiempo que a transcurrido al total

            yield return null; //regresa una instancia, regresa un nulo, ya termino d contsr
        }

        //el evento a ejecutar
        onFillComplete?.Invoke(); //ya se completo el evento, el ?, evalua que tiene onFillComplete


    }

    // Update is called once per frame
    void Update() {

    }
}