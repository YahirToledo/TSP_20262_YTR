using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Receiver.Primitives;
using System.IO;

public class FlightThreadSinc : MonoBehaviour {
    public float speed = 50f;
    public float rotationSpeed = 100f;
    public Transform cameraTransform;
    public Vector2 movementInput;

    //Control de iteraciones
    public int turbulenceIterations = 1000000;

    //Lista de vectores de posicion calculados
    private List<Vector3> turbulenceForces = new List<Vector3>();

    //Variales para manipular el hilo secundario
    private Thread turbulenceThread; //La instancia del hilo secudario
    private bool isTurbulenceRunning = false; //Bandera para saber si sigue el calculo
    private bool stopTurbulenceThread = false; //Bandera para saber si el hilo termino
    private float capturedTime; //Variable para alamcenar el tiempo transcurrido

    //Bandera de control sobre lectura
    public bool read = false;
    public bool write = false;
    private object filelock = new object();
    //Ruta de almacenamiento
    string filepath;

    //Metodo para
    public void OnMovement(InputValue value) {
        movementInput = value.Get<Vector2>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        filepath = Application.dataPath + "/TurbulenceData.txt";
        Debug.Log("Ruta al archivo" + filepath);
    }

    // Update is called once per frame
    void Update() {
        if (cameraTransform == null) {
            Debug.LogError("No hay camra asignada");
            return;
        }

        //Tiemp otranscurrido
        capturedTime = Time.time;

        //Proceso pesado en hilo secundario
        if (!isTurbulenceRunning) {
            isTurbulenceRunning = true;
            stopTurbulenceThread = false;

            turbulenceThread = new Thread(() =>
            SimulateTurbulence(capturedTime));
            turbulenceThread.Start();
        }

        //Mover la nave de manera linealmente
        Vector3 moveDirecton = cameraTransform.forward * movementInput.y * speed * Time.deltaTime;
        this.transform.position += moveDirecton;

        //Mover la nave en rotacion
        float yaw = movementInput.x * rotationSpeed * Time.deltaTime;
        this.transform.Rotate(0, yaw, 0);

        //ACTIVIDAD 3: Sincronizar hilos
        if (write && !read) 
        {
            TryReadFile();
            read = true;
        }
    }

    public void SimulateTurbulence(float time) {
        turbulenceForces.Clear();
        //Repeticiones
        for (int i = 0; i < turbulenceIterations; i++) {
            //Verificar si se debe detener el hilo
            if (stopTurbulenceThread) {
                break;
            }
            Vector3 force = new Vector3(
                    Mathf.PerlinNoise(i * 0.001f, time) * 2 - 1,
                    Mathf.PerlinNoise(i * 0.002f, time) * 2 - 1,
                    Mathf.PerlinNoise(i * 0.003f, time) * 2 - 1
                );
            turbulenceForces.Add(force);
        }

        //Señal en consola de inicio del hilo
        Debug.Log("Iniciando simulacion de turbulencia");

        Debug.Log("Escribiendo archivo");

        //ACTIVIDAD 3: Metodo para lectura del archivo
        //Escritura del archivo

        lock (filelock) {
            //Escritura del archivo
            using (StreamWriter writer = new StreamWriter(filepath, false)) {
                foreach (var force in turbulenceForces) {
                    writer.WriteLine(force.ToString());
                }
                writer.Flush();
            }
        }

        Debug.Log("Archivo escrito");

        //Simulacion completa
        isTurbulenceRunning = false;
        write = true;  
    }
    void TryReadFile() {
        try {
            lock (filelock) 
            {
                if (File.Exists(filepath)) 
                {
                    string content = File.ReadAllText(filepath);
                    Debug.Log("Archivo leido " + content);
                } 
                else 
                {
                    Debug.LogError("Ocurrio un problema");
                }
            }
        } catch (IOException ex) {
            Debug.LogError("Error de acceso a archivo " + ex.Message);
        }
    }
    private void OnDestroy() {
        //Indicar el cierre del hilo secundario
        stopTurbulenceThread = true;

        //Verificar si el hilo existe y se esa ejecutando
        if (turbulenceThread != null && turbulenceThread.IsAlive) {
            //Lo unimos al hilo principal y cerramos la ejecucion
            turbulenceThread.Join();
        }
    }
}
