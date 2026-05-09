using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public GameObject playerPrefab;
    public Transform playerSpawnPoint;
    public GameObject enemiesPrefab;
    public Transform[] enemiesSpawnPoint;
    public int numberOfEnemies = 3;

    public GameObject PlayerInstance;
    public GameObject[] EnemiesInstance;

    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }

    // Called when the AR target is found
    public void OnTargetFound() { 
        Debug.Log("Target found, spawning player and enemies.");
        Invoke(nameof(SpawnPlayer), 5f); // Spawn player in 5 seconds
        InvokeRepeating(nameof(SpawnEnemies), 5f, 10f); // Spawn enemies every 10 seconds after 5 seconds
    }

    // Called when the AR target is lost
    public void OnTargetLost() { 
        Debug.Log("Target lost, cleaning up player and enemies.");
        if (PlayerInstance != null) { 
            Destroy(PlayerInstance);
        }
        if(EnemiesInstance != null) { 
            foreach(var enemy in EnemiesInstance) { 
                if(enemy != null) { 
                    Destroy(enemy);
                }
            }
        }
    }

    private void SpawnPlayer() { 
        PlayerInstance = Instantiate(playerPrefab, 
            playerSpawnPoint.position, playerSpawnPoint.rotation);
    }

    private void SpawnEnemies() {
        EnemiesInstance = new GameObject[numberOfEnemies];
        for(int i = 0; i < numberOfEnemies; i++) { 
            EnemiesInstance[i] = Instantiate(enemiesPrefab,
                enemiesSpawnPoint[i].position, enemiesSpawnPoint[i].rotation);
        }
    }
}
