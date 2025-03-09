using UnityEngine;

public class DestroyBulletOnCollision : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<FireBulletsAtTarget>()) return;
        
        Destroy(gameObject);
    }
}
