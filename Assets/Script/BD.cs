using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Firebase.Database;
using Firebase.Extensions;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class BD : MonoBehaviour {
    public DatabaseReference reference;
    [SerializeField]
    TMP_InputField textoNombre;
    [SerializeField]
    TMP_InputField textoEdad;
    public bool registroBooleano = false;

    private void Awake() {
        reference = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void Booleano(bool toggledB) {
        registroBooleano = true;
    }

    public void Registro() {
        //Generar clave para el registro tipo string
        string key = reference.Child("Nombre").Push().Key;
        reference.Child("Nombre").Child(key).SetValueAsync(textoNombre.text);

        //Clave unica para datos individuales tipo int
        reference.Child("Edad").SetValueAsync(int.Parse(textoEdad.text));

        //Clave para tipo booleano
        reference.Child("Booleano").SetValueAsync(registroBooleano);

        //Clave para registro de objeto tipo usuario
        Usuario usuario = new Usuario("Mario", "mario@gmail.com");
        string json = JsonUtility.ToJson(usuario);

        reference.Child("Usuario").SetRawJsonValueAsync(json);

        //Datos a actualizar
        Debug.Log("Dato Escuela anterior: UNAM");
        reference.Child("Escuela").SetValueAsync("UNAM");

        Debug.Log("Dato Escuela actual: POLI");
        reference.Child("Escuela").SetValueAsync("POLI");


        Debug.Log("Dato Año anterior: 2025");
        reference.Child("Año").SetValueAsync(2025);

        Debug.Log("Dato Año actual: 2026");
        reference.Child("Año").SetValueAsync(2026);
    }

    public void CargaBD() {
        //Obteniendo registro año
        reference.Child("Año").GetValueAsync().ContinueWithOnMainThread(TaskExtension => 
            {
            if (TaskExtension.IsFaulted) {
                Debug.Log("Error al obtener datos" + TaskExtension.Exception);
            } 
            else if (TaskExtension.IsCompleted) {
                DataSnapshot snapshot = TaskExtension.Result;
                string value = snapshot.Value.ToString();
                //Debug.Log($"{value}");
                Debug.Log("Tipo de valor obtenido " + snapshot.Value.GetType());
                Debug.Log("Valor: " + value);
            }
            else {
                    Debug.Log("Registro con error");
            }
        });

        //Carga de valores anidados con clave unica
        reference.Child("Nombre").GetValueAsync().ContinueWithOnMainThread(TaskExtension => {
            if (TaskExtension.IsFaulted) {
                Debug.Log("Error al obtener datos" + TaskExtension.Exception);
            } 
            else if (TaskExtension.IsCompleted) {
                DataSnapshot snapshot2 = TaskExtension.Result;
                string value = snapshot2.Value.ToString();
                //Debug.Log($"{value}");
                Debug.Log("Tipo de valor obtenido " + snapshot2.Value.GetType());
                Debug.Log("Valor: " + value);
            } 
            else {
                Debug.Log("Registro con error");
            }
        }
            
        );

    }

}

public class Usuario {
    public string UserName;
    public string Email;

    public Usuario(string userName, string email) {
        this.UserName = userName;
        this.Email = email;
    }
}