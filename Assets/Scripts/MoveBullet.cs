using UnityEngine;

public class MoveBullet : MonoBehaviour
{
    private Transform target;
    private float _speed = 1;
    private bool isHoming = false;

    // Update is called once per frame
    void Update()
    {
        if (isHoming)
        {
            transform.LookAt(target.position);
        }

        transform.Translate(Vector3.forward * (Time.deltaTime * _speed));
        

    }

    public void Configure(float speed, bool homingBullets, Transform NewHomingTarget)
    {
        _speed = speed;
        target = NewHomingTarget;
        isHoming = homingBullets;
        transform.LookAt(target.position);
        Destroy(gameObject, 3);
    }

}
