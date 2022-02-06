using UnityEngine;
using UnityEngine.Events;

public enum CollectibleType
{
    Corpse,
    Stopwatch,
    Jump,
    Key,
    Other
}

public class Collectible : MonoBehaviour
{
    [Tooltip("Controls if we randomize the collectible color or not.")]
    public bool RandomColor = false;
    [Tooltip("How fast the collectible rotates.")]
    public float RotationSpeed;
    [Tooltip("If collectible type is associated with a value, controls how much we increase that value by.")]
    public float CollectibleAmount = 1.0f;
    [Tooltip("What type of collectible this is.")]
    public CollectibleType type = CollectibleType.Corpse;

    [Tooltip("Event that gets called when the collectible is collected.")]
    public UnityEvent OnActivate;

    // Set the collectible to a random color on start if RandomColor flag is active.
    void Start()
    {
        if(RandomColor)
        {
            GetComponentInChildren<MeshRenderer>().materials[0].color = new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f));
        }
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * RotationSpeed, 0));
    }

    // If player enters the collectible trigger, call ObtainCollectible function.
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            ObtainCollectible(player);
        }
    }

    // Function that is called when collectible is collected. Increases various values depending on the collectible type. Also destroys the collectible gameobject.
    private void ObtainCollectible(PlayerController player)
    {
        switch (type)
        {
            case CollectibleType.Corpse:
                player.CorpseLimit += (int)CollectibleAmount;
                break;
            case CollectibleType.Stopwatch:
                player.timer += CollectibleAmount;
                player.RespawnTime += CollectibleAmount;
                break;
            case CollectibleType.Jump:
                player.JumpCount += (int)CollectibleAmount;
                player.availableJumpCount += (int)CollectibleAmount;
                break;
            case CollectibleType.Key:
                player.keys++;
                break;
            case CollectibleType.Other:
                OnActivate.Invoke();
                break;
        }
        player.Sounds.PlayOneShot(player.ItemSound);
        Destroy(gameObject);
    }
}
