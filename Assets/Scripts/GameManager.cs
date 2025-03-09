using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    #region Turrets
    [SerializeField] GameObject[] turretsPrefab;
    [SerializeField] int numberOfTurrets = 5;
    private GameObject[] _turrets;

    [SerializeField] [Range(0.1f, 50f)] private float minDistanceX = 10;
    [SerializeField] [Range(0.1f, 50f)] private float minDistanceZ = 10;
    
    [SerializeField] [Range(0.1f, 100f)] private float deltaX = 30;
    [SerializeField] [Range(0.1f, 100f)] private float deltaZ = 30;

    [SerializeField] [Range(0.05f,5f)] private float minFireRate = 0.5f;
    [SerializeField] [Range(0.05f,5f)] private float maxFireRate = 2f;
    
    [SerializeField] [Range(1f,50f)]  private float minFireDistance = 10f;
    [SerializeField] [Range(1f,100f)] private float maxFireDistance = 20f;
    #endregion

    #region Walls
    
    private int _wallsAvailable = 0;
    
    [SerializeField] private GameObject[] wallsPrefab;
   
    [SerializeField] [Range(0.1f, 50f)] private float minDistanceWallX = 1;
    [SerializeField] [Range(0.1f, 50f)] private float minDistanceWallZ = 5;
    
    [SerializeField] [Range(0.1f, 100f)] private float deltaWallX = 1;
    [SerializeField] [Range(0.1f, 100f)] private float deltaWallZ = 1;
    #endregion

    [SerializeField] GameObject gameOver;
    [SerializeField] GameObject youWin;

    void Start()
    {
        if (numberOfTurrets == 0)
        {
            Debug.LogWarning("No number of turrets detected");
            return;
        }
        
        _wallsAvailable = numberOfTurrets;
        
        _turrets = new GameObject[numberOfTurrets];

        for (int i = 0; i < numberOfTurrets; i++)
        {
            GameObject turret = Instantiate(turretsPrefab[Random.Range(0, turretsPrefab.Length)]);
            _turrets[i] = turret;

            int tries = 5;

            bool intersect = false;
            do
            {
                turret.transform.position = new Vector3(minDistanceX + Random.Range(-1f, 1f) * deltaX, 0,
                    minDistanceZ + Random.Range(-1f, 1f) * deltaZ);
                turret.transform.Rotate(Vector3.up, Random.Range(0f, 360f), Space.World);

                foreach (var addedTurret in _turrets)
                {
                    if (addedTurret ==null || addedTurret == turret) continue;
                    
                    if (addedTurret.GetComponent<CapsuleCollider>().bounds.Intersects(turret.GetComponent<CapsuleCollider>().bounds))
                    {
                        intersect = true;
                        break;
                    }
                }

                tries--;

            } while (intersect && tries > 0);

            FireBulletsAtTarget turretScript = turret.GetComponent<FireBulletsAtTarget>();
            turretScript.Configure(Random.Range(minFireRate, maxFireRate),
                Random.Range(minFireDistance, maxFireDistance),
                transform);

            //33% of homing bullet turret type
            if (Random.Range(0, 11) > 7)
            {
                turretScript.SetHoming(true);
            }

            //Place wall around turret

            GameObject wall = Instantiate(wallsPrefab[Random.Range(0, wallsPrefab.Length)]);

            wall.transform.position = turret.transform.position + new Vector3(
                minDistanceWallX + Random.Range(-1f, 1f) * deltaWallX,
                wall.transform.localScale.y * 0.5f,
                minDistanceWallZ + Random.Range(-1f, 1f) * deltaWallZ);
           
            //rotate around turret randomly
            wall.transform.RotateAround(turret.transform.position, Vector3.up, Random.Range(0f, 360f));

            //rotate around itself randomly locally
            wall.transform.Rotate(Vector3.up, Random.Range(-45f, 45f), Space.Self);
            
            //We want to be notified (better to use (next lessons) -> delegates, actions, function etc)
            DestroyOnMultipleHit destroyOnMultipleHit = wall.GetComponent<DestroyOnMultipleHit>();
            destroyOnMultipleHit.GameManager = this; //-> DidDestroyWall
        }
    }

    public void GameOver()
    {
        DestroyAllTurrets();
        
        Debug.Log($"Game OVER: Play time: {Time.time}");
        
        gameOver.SetActive(true);
    }
    public void DidDestroyWall()
    {
        _wallsAvailable--;

        if (_wallsAvailable <= 0)
        {
            DestroyAllTurrets();

            Debug.Log($"Game OVER: YOU WIN! Play time: {Time.time}");
            
            youWin.SetActive(true);
        }
    }
    
    private void DestroyAllTurrets()
    {
        foreach (var turret in _turrets)
        {
            Destroy(turret);
        }
    }

    private void Update()
    {
        //Debug.Log($"Vertical: {Input.GetAxis("Vertical")}");
    }
}
