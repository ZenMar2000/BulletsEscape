using UnityEngine;
using Random = UnityEngine.Random;

public class DestroyOnMultipleHit : MonoBehaviour {
    [SerializeField] int maxHitCount = 10;
    [SerializeField] private bool randomHitCount = true;

    //Used to change color/transparency
    Material _material;
    private float _destroyStepsPercent = 1;

    //Used to notify about destructions
    GameManager _gameManager;
    public GameManager GameManager { set => _gameManager = value; }

    private AudioSource _audioSource;

    private void Start()
    {
        _material = GetComponent<MeshRenderer>().material;

        _audioSource = GetComponent<AudioSource>();
        
        if (randomHitCount)
        {
            maxHitCount = Random.Range(1,maxHitCount); //note max value is hitcount-1
        }

        _destroyStepsPercent = 1f/maxHitCount; 
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.GetComponent<MoveBullet>()) return;

        maxHitCount -= 1;

        _material.color -= new Color(0, 0, 0, _destroyStepsPercent);
        
        Debug.Log($"{_destroyStepsPercent} -> alpha = {_material.color.a}");

        if (maxHitCount <= 0)
        {
            _gameManager.DidDestroyWall();
            
            if (_audioSource.clip)
            {
                _audioSource.Play();
                Invoke(nameof(DestroyMe), _audioSource.clip.length); //Play audio before destroying
            }
            else
            {
                Destroy(gameObject);
            }
            
            //To avoid next calls (we can't disable component 'cause OnCollision & OnTrigger are always called
            //even if the component is disabled
            Destroy(this);
            
        }
    }

    private void DestroyMe()
    {
        _gameManager = null;
        Destroy(gameObject);
    }
}
