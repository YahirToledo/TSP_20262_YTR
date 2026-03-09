using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;


public class Concurrencia : MonoBehaviour
{
    //Header es para hacer los encabezados en las propiedades de un objeto
    [Header("Activa los metodos")]
    public bool useSincrono;
    public bool useThread;
    public bool useTask;
    public bool useCoroutine;

    [Header("Esfera a mover")]
    public Transform sincronorSphere;
    public Transform threadSphere;
    public Transform taskSphere;
    public Transform coroutineSphere;
    public Transform mainCube;

    //Acciones a ejecutar en el hilo secundario
    private Queue<Action> mainThreadActions = new Queue<Action>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Siemre gira el cubo de referencia
        mainCube.Rotate(Vector3.up, 50*Time.deltaTime);

        //Ejecuta las acciones en el hilo principal
        lock (mainThreadActions) 
        {
            while (mainThreadActions.Count > 0) 
            {
                mainThreadActions.Dequeue().Invoke();
            }
        }
    }

    //Metodos para herramientas de concurrencia
    void MoveSincrono() 
    {
        for (int i =0; i<=100; i++) 
        {
            sincronorSphere.position += Vector3.right * 0.05f;
        }
        Thread.Sleep(50);
    }

    //Corutina
    IEnumerator MoveWithCoroutine() 
    {
        for (int i = 0; i <= 100; i++) {
            coroutineSphere.position += Vector3.right * 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
