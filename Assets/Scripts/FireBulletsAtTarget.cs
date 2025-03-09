using UnityEngine;

public class FireBulletsAtTarget : MonoBehaviour
{
    [SerializeField] GameObject activationMark;
    [SerializeField] Material normalActivationMark;
    [SerializeField] Material homingActivationMark;

    [SerializeField] GameObject bulletPrefab;
    [SerializeField] GameObject head;
    [SerializeField] Transform[] firePositions;
    [SerializeField][Range(1f, 10f)] private float minBulletSpeed;
    [SerializeField][Range(1f, 10f)] private float maxBulletSpeed;
    [SerializeField][Range(1f, 180f)] private float rotateSpeed = 45;
    [SerializeField][Range(-1f, 1)] private float fireAngle = -0.5f;
    [SerializeField] private bool homingBullets = false;

    private Transform _target;

    private float _fireRate = 1;
    private float _fireDistance = 1;
    private float nextFire;
    private AudioSource _aso;

    [SerializeField] private bool debugDetectionAngle = false;
    [SerializeField] private bool debugArea = false;

    public void Configure(float fireRate, float fireDistance, Transform trackTarget)
    {
        _target = trackTarget;
        _fireRate = fireRate;
        _fireDistance = fireDistance;
    }

    private void Start()
    {
        _aso = GetComponentInParent<AudioSource>();

        activationMark.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!_target) return;

        if (Vector3.Distance(transform.position, _target.position) > _fireDistance)
        {
            head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, Quaternion.identity, rotateSpeed * Time.deltaTime);
            activationMark.SetActive(false);
            return;
        }

        activationMark.SetActive(true);

        Vector3 directionStart = firePositions[0].position.normalized;
        Vector3 directionEnd = (firePositions[0].transform.position - _target.transform.position).normalized;

        float angle = Vector3.Angle(directionStart, directionEnd);

        float dot = Vector3.Dot(directionStart, directionEnd);

        if (debugDetectionAngle) Debug.LogWarning($"{gameObject.name}: {angle} dot: {dot}", gameObject);

        var destRotation = Quaternion.LookRotation(_target.position - head.transform.position);
        head.transform.rotation = Quaternion.RotateTowards(head.transform.rotation, destRotation, rotateSpeed * Time.deltaTime);

        if (dot > fireAngle) return;

        nextFire += Time.deltaTime;

        if (nextFire >= _fireRate)
        {

            foreach (Transform firePosition in firePositions)
            {
                GameObject bullet = Instantiate(bulletPrefab, firePosition.position, Quaternion.Euler(90, 0, 0));
                if (homingBullets)
                {
                    bullet.GetComponent<MoveBullet>().Configure(Random.Range(minBulletSpeed, maxBulletSpeed), true, _target);
                }
                else
                {
                    bullet.GetComponent<MoveBullet>().Configure(Random.Range(minBulletSpeed, maxBulletSpeed), false, _target);
                }

                //bullet.transform.rotation = Quaternion.LookRotation(Vector3.up, _target.position - firePosition.position);
                bullet.transform.forward = _target.position - firePosition.position;
            }

            _aso.Play();

            nextFire = 0f;
        }


    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || !debugArea) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _fireDistance);
    }

    public void SetHoming(bool hasHomingBullets)
    {
        homingBullets = hasHomingBullets;
        if (homingBullets)
        {
            activationMark.GetComponent<ChangeMaterial>().ChangeCurrentMaterial(homingActivationMark);
        }
        else
        {
            activationMark.GetComponent<ChangeMaterial>().ChangeCurrentMaterial(normalActivationMark);
        }

    }
}
