//Script para: Cosas que pueden pasar cuando presionamos el boton
using NUnit.Framework; //en esta version es necesaria, pero si se quita y funciona, hay que quitarla jaja
using System.Collections.Generic; //para usar listas (agregar Collections.Generic)
using UnityEngine;
using UnityEngine.SceneManagement; //libreria para manejo de escenas
using TMPro;

public class EventUI : MonoBehaviour {
    //estructuras que permiten almacenar,como tenermos objetos panel, en este caso usar lista(permite que sea de cualquier tipo)
    public List<GameObject> listaDeInstrucciones; //game object, instancias que ya estan en el juego, en este caso paneles
    public int currentIndex = 0;
    public List<string> mensajesDeInstrucciones;
    public TextMeshProUGUI textMeshProUGUI;

    //la diferencua
    private void Awake()  //dice configuraciones que n deba volver a toacar despues, guarda config que se quieren conservar
    {
        //por ejemplo no destuir un objeto sin importar si se cambia de escena
        DontDestroyOnLoad(this.gameObject); //cuando la clase hace una instancia denro de un objeto, This hace referencia al objeto controlador

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {
        //Actulizar visibilidad de paneles
        UpdateVisibilty();
    }

    // Update is called once per frame
    void Update() {

    }

    //Metodo para actualizar visibilidad de paneles
    private void UpdateVisibilty() {
        //apuntador: indice, se debe de acceder al indice, para saber cuantos hay y en cual estoy actualmente
        for (int i = 0; i < listaDeInstrucciones.Count; i++)  //uno menor a los objetos que tiene
        {
            //Solo el panel en el indice actual esta aqui
            listaDeInstrucciones[i].SetActive(i == currentIndex);//solo el del indice actual es el que se debe ver, no los demas
                                                                 //condicionado a que solo sera verdadero cuando i sea igual al indice actual
        }
    }

    //Metodo para cambiar de escena, con numero
    public void ChangeSceneByIndex(int sceneIndex) {
        SceneManager.LoadScene(sceneIndex);
    }

    //Metodo para cambiar de escena, por nombre de la escena
    public void ChangeSceneByName(string sceneName) {
        SceneManager.LoadScene(sceneName);
    }

    //Recargar esce a actual
    public void ReloadCurrentScene() 
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }


    //metodo, aumena el indice y cambia entre paneles
    //metodo para cambiar entre paneles
    public void CycleObject(int direction) {
        //operacion del modulo: permite establecer un rango, dice si una division es perfecta o no, si es perfecta regresa a 0
        //incrementa el indice y vuelve al principio
        currentIndex = (currentIndex + direction + listaDeInstrucciones.Count) % listaDeInstrucciones.Count; //cambia el indice al siguiente del actual

        //Actualizar la visibilidad
        UpdateVisibilty();
    }

    //metodo para actualizar el texto mostrado
    public void UpdateText() {
        if (mensajesDeInstrucciones.Count > 0 && textMeshProUGUI != null) 
        {
            textMeshProUGUI.text = mensajesDeInstrucciones[currentIndex];
        }
    }

    public void CycleText(int direction) {
        //operacion del modulo: permite establecer un rango, dice si una division es perfecta o no, si es perfecta regresa a 0
        //incrementa el indice y vuelve al principio
        currentIndex = (currentIndex + direction + mensajesDeInstrucciones.Count) % mensajesDeInstrucciones.Count; //cambia el indice al siguiente del actual

        //Actualizar la visibilidad
        UpdateText();
    }

    //metodo para salir de la aplicacion
    public void ExitGame() {
        //donde vas a hacer la impresion del mensaje
        Debug.Log("Va a salir");
        Application.Quit();
        Debug.Log("Ya salio");
    }


}