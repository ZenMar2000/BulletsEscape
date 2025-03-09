using System;
using UnityEngine;

public class GameOverOnCollision : MonoBehaviour
{
    GameManager _turretsManager;

    private void Start()
    {
        _turretsManager = GetComponent<GameManager>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.GetComponent<MoveBullet>()) return;
        _turretsManager.GameOver();
        Destroy(GetComponent<Collider>()); //Destroy collider to stop being hit
    }
}
