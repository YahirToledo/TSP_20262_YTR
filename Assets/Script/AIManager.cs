using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using Unity.VisualScripting;

public class AIManager : MonoBehaviour
{
    [SerializeField]
    public GameObject player;
    [SerializeField]
    public Transform entrance;
    [SerializeField]
    public Transform exit;

    public float detectionRange = 2.0f;
    public float exitRange = 2f;
    public float minDistanceFromEntrance = 15f;

    List<NavMeshAgent> agents = new();

    NavMeshTriangulation triangulation;
    Vector3 entrancePos;

    float detectionRangeSqr;
    float exitRangeSqr;
    float minDistanceSqr;

    bool gameWon = false;

    System.Random random = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entrancePos = entrance.position;
        triangulation = NavMesh.CalculateTriangulation();

        detectionRangeSqr = detectionRange*detectionRange;
        exitRangeSqr = Mathf.Pow(exitRange, 2);
        minDistanceSqr = Mathf.Pow(minDistanceFromEntrance, 2);

        FindAllEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 playerPos = player.transform.position;

        //Deteccion de enemigos en posicion de jugador
        bool playerCaught = false;
        foreach (var agent in agents) 
        {
            if (!agent.enabled) continue;
            if ((agent.transform.position - player.transform.position).sqrMagnitude < detectionRangeSqr)
            {
                playerCaught = true;
                break;
            }
        }
    }

    //Metodo ara llevar a player a la entrada
    void TeleportPlayerToEntrance()
    {
        var cc = player.GetComponent<NavMeshAgent>();
        if (cc != null) 
        {
            cc.enabled =false;
        }
        player.transform.position = entrancePos;
        if (cc != null)
        {
            cc.enabled = true;
        }
        Debug.Log("Teletransport: " + entrancePos);
    }

    //Metodos para posicionar a los enemigos
    void RelocateAllNPC()
    {
        if (triangulation.vertices.Length == 0) return;
        
        foreach (var agent in agents)
        {
            agent.enabled = false;
            agent.transform.position = GetValidRandomPosition();
            agent.enabled = true;
        }
    }

    //Metodo para calcular posiciones validas
    Vector3 GetValidRandomPosition()
    {
        Vector3 pos;
        do
        {
            int i = random.Next(0,triangulation.indices.Length / 3) * 3;
            
            Vector3 v1 = triangulation.vertices[triangulation.indices[i]];
            Vector3 v2 = triangulation.vertices[triangulation.indices[i+1]];
            Vector3 v3 = triangulation.vertices[triangulation.indices[i+2]];

            float r1 = (float)random.NextDouble();
            float r2 = (float)random.NextDouble();

            if (r1 + r2 > 1) 
            {
                r1 = 1f - r1;
                r2 = 1f - r2;
            }

            pos = v1 + r1 * (v2 - v1) + r2 * (v3 - v1);
        } while ((pos - entrancePos).sqrMagnitude < minDistanceSqr);
        return pos;
    }

    //Metodo para ubicar los objetos enemigo
    void FindAllEnemies()
    {
        agents.Clear();
        foreach (var agent in FindObjectsByType<NavMeshAgent>(FindObjectsSortMode.None)) 
        {
            if (agent.CompareTag("Enemy")) 
            {
                agents.Add(agent);
            }
        }
    }
}
