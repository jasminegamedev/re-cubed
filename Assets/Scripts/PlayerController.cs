using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// This is the main player controller that handles moving, jumping, and dieing, as well as other player actions.
public class PlayerController : MonoBehaviour
{
    [Tooltip("How many seconds until the player dies.")]
    public float RespawnTime = 15.0f;
    [Tooltip("How fast the player moves.")]
    public float Speed = 100;
    [Tooltip("How high the player can jump.")]
    public float JumpForce = 100;
    [Tooltip("How many corpses the player can leave behind before they start to destroy themselves.")]
    public int CorpseLimit = 0;
    [Tooltip("How many times the player can jump.")]
    public int JumpCount = 0;

    [Tooltip("Prefab that spawns when the player dies.")]
    public GameObject CorpsePrefab;
    [Tooltip("Sound that plays when the player jumps.")]
    public AudioClip JumpSound;
    [Tooltip("Sound that plays when an item is collected.")]
    public AudioClip ItemSound;

    //How long is currently on the timer.
    public float timer { get; set; }
    // How many jumps we currently have available
    public int availableJumpCount { get; set; }
    // If we should currently be updating the timer
    public bool isTimerPaused { get; set; }
    // Number of Collected keys
    public int keys { get; set; }
    // Current Spawn point.
    public GameObject SpawnPoint { get; set; }
    // Audio Source used for sound effects
    public AudioSource Sounds { get; set; }

    // Player Color
    private Color _bodyColor;
    // Distance to the edge of the collider
    private float distToEdge;
    // If we should check if the player is on ground
    private bool checkGrounded = true;

    // Cached list of corpses
    private System.Collections.Generic.List<GameObject> _corpses = new System.Collections.Generic.List<GameObject>();

    // Cached Components
    private Rigidbody _rbody;
    private MeshRenderer _bodyMesh;

    // Cached Text Components
    private Text _timerText;
    private Text _maxTimerText;
    private Text _corpseText;
    private Text _jumpText;
    private Text _keyText;


    void Start()
    {
        timer = RespawnTime;
        _rbody = GetComponent<Rigidbody>();
        _bodyMesh = GetComponentInChildren<MeshRenderer>();
        _bodyColor = new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f));
        _bodyMesh.materials[0].color = _bodyColor;
        distToEdge = GetComponent<BoxCollider>().bounds.extents.y;

        _timerText = GameObject.Find("TimerText").GetComponent<Text>();
        _maxTimerText = GameObject.Find("MaxTimerText").GetComponent<Text>();
        _corpseText = GameObject.Find("CorpseText").GetComponent<Text>();
        _jumpText = GameObject.Find("JumpText").GetComponent<Text>();
        _keyText = GameObject.Find("KeyText").GetComponent<Text>();
        availableJumpCount = JumpCount;

        Sounds = GetComponent<AudioSource>();
    }

    void Update()
    {
        // "Kill" player if timer is over or if player forces it.
        if (!isTimerPaused)
        {
            timer -= Time.deltaTime;

            if (timer <= 0)
            {
                FakeDie();
            }
            else if (Input.GetButtonDown("Fire3") || Input.GetMouseButtonDown(1))
            {
                FakeDie();
            }
        }

        // Set text values
        _timerText.text = string.Format("{0:0.00}", timer);
        _maxTimerText.text = string.Format("x{0:0.00}", RespawnTime);
        _corpseText.text = string.Format("x{0}", CorpseLimit);
        _keyText.text = string.Format("x{0}", keys);
        _jumpText.text = string.Format("x{0}", JumpCount);

        if (checkGrounded && IsGrounded())
        {
            availableJumpCount = JumpCount;
            checkGrounded = false;
        }

        // Handle jumping
        if ((Input.GetButtonDown("Jump") || Input.GetMouseButtonDown(0)) && (IsGrounded() || availableJumpCount > 0))
        {
            if (availableJumpCount > 0)
            {
                StartCoroutine(ResetCheckGrounded());
                _rbody.velocity = new Vector3(_rbody.velocity.x, 0, _rbody.velocity.z);
                _rbody.AddForce(transform.up * JumpForce, ForceMode.Impulse);
                availableJumpCount--;

                Sounds.PlayOneShot(JumpSound);
            }
        }
    }

    IEnumerator ResetCheckGrounded()
    {
        yield return new WaitForSeconds(0.02f);
        checkGrounded = true;
    }

    // Checks if player is standing on ground.
    private bool IsGrounded()
    {
        bool isGrounded = Mathf.Abs(_rbody.velocity.y) < 0.05f && (Physics.Raycast(transform.position, -Vector3.up, distToEdge + 0.2f) ||
            Physics.Raycast(transform.position + new Vector3(distToEdge -0.02f, 0, distToEdge - 0.02f), -Vector3.up, distToEdge + 0.2f) ||
            Physics.Raycast(transform.position + new Vector3(0.02f - distToEdge, 0, distToEdge - 0.02f), -Vector3.up, distToEdge + 0.2f) ||
            Physics.Raycast(transform.position + new Vector3(distToEdge - 0.02f, 0, 0.02f - distToEdge), -Vector3.up, distToEdge + 0.2f) ||
            Physics.Raycast(transform.position + new Vector3(0.02f - distToEdge, 0, 0.02f - distToEdge), -Vector3.up, distToEdge + 0.2f));

        return isGrounded;
    }

    // We don't actually want to destroy the game object on death. Instead, we spawn a corpse gameobject, and teleport the player back to the spawn point.
    // Also delete old corpses when we have too many.
    public void FakeDie()
    {
        timer = RespawnTime;
        var previousPosition = transform.position;
        transform.position = SpawnPoint.transform.position + transform.up;
        transform.rotation = Quaternion.Euler(transform.forward);
        _rbody.velocity = Vector3.zero;

        var corpse = Instantiate(CorpsePrefab, previousPosition, transform.rotation);
        corpse.GetComponentInChildren<MeshRenderer>().materials[0].color = _bodyColor;

        _bodyColor = new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f));
        _bodyMesh.materials[0].color = _bodyColor;
        _corpses.Add(corpse);
        if(_corpses.Count > CorpseLimit)
        {
            var oldCorpse = _corpses[0];
            _corpses.RemoveAt(0);
            oldCorpse.transform.position = new Vector3(-1000, -1000, -1000);
            StartCoroutine(DelayedDestroy(oldCorpse, 0.1f));
        }
    }    

    // Destroy object after a given period of time.
    private IEnumerator DelayedDestroy(GameObject destroyObject, float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(destroyObject);
    }

    // Handle movement by setting velocity
    private void FixedUpdate()
    {
        float fall = _rbody.velocity.y;

        if (Mathf.Abs(Input.GetAxis("Horizontal")) + Mathf.Abs(Input.GetAxis("Vertical")) > 0.1f)
        {
             transform.LookAt(transform.position - new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")));

            _rbody.velocity = -transform.forward * Speed * Time.deltaTime;
        }
        else
        {
            _rbody.velocity = Vector3.zero;
        }
        _rbody.velocity = new Vector3(_rbody.velocity.x, fall, _rbody.velocity.z);
    }
}
