using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField][Range(0.1f, 10)] private float movementSpeed = 0.5f;
    [SerializeField][Range(1, 180)] private float rotationSpeed = 45;
    [SerializeField][Range(1, 100)] private float mouseSpeed = 8f;
    [SerializeField][Range(1, 10)] private float turboSpeed = 2f;
    [SerializeField] GameObject wallPrefab;

    [SerializeField] bool useMouseLook = true;
    [SerializeField] CursorLockMode useLockState = CursorLockMode.Locked;

    private float _turbo;
    private float _h;
    private float _v;

    private float wallPlacementCooldown = 0f;

    private void Start()
    {
        if (useMouseLook)
        {
            Cursor.lockState = useLockState;
        }
    }

    void Update()
    {
        _turbo = (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) ? turboSpeed : 1;

        if (Input.GetKeyDown(KeyCode.E)) PlacePlayerWall();

        float mouse = Input.GetAxis("Mouse X");
        if (wallPlacementCooldown > 0)
        {
            wallPlacementCooldown -= Time.deltaTime;
        }
        //Debug.LogWarning(mouse);

        _h = useMouseLook ? mouse : Input.GetAxis("Horizontal");
        _v = Input.GetAxis("Vertical");

        float xDirection = useMouseLook ? Input.GetAxis("Horizontal") : 0;
        float zDirection = _v /** movementSpeed*/;

        Vector3 direction = new Vector3(xDirection, 0, zDirection).normalized * movementSpeed * (_turbo * Time.deltaTime); //move

        transform.Translate(direction);

        if (useMouseLook)
        {
            transform.Rotate(new Vector3(0, mouse * mouseSpeed * Time.deltaTime, 0));
        }
        else // sx/dx or a/d
        {
            transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime * _h * _turbo));
        }
    }

    private void PlacePlayerWall()
    {
        if (wallPlacementCooldown <= 0)
        {
            GameObject wall = Instantiate(wallPrefab);
            wall.transform.position = transform.position + transform.forward * 2;
            wall.transform.rotation = transform.rotation;
            wallPlacementCooldown = 3;
        }
    }

    /*private void FixedUpdate()
    {
        if (!useRigidBody) return;
        
       // _rb.MovePosition(_rb.position + (_rb.rotation * new Vector3(h,0,v).normalized) * (mSpeed * turbo * Time.fixedDeltaTime));
        _rb.Move(_rb.position + transform.forward * (v * mSpeed * Time.fixedDeltaTime * turbo),
            _rb.rotation * Quaternion.Euler(0,rotationSpeed * Time.fixedDeltaTime * h * turbo,0));
        
    }*/
}
