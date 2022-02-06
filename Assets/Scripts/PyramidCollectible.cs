using UnityEngine;
using UnityEngine.SceneManagement;

// This component represents the pyramid collectible, which is the main goal in each level.
// When the player touches this, it will send them to the next level.
public class PyramidCollectible : MonoBehaviour
{
    [Tooltip("Speed to rotate the collectible.")]
    public float RotationSpeed;

    [Tooltip("Scene to load into when collectible is grabbed.")]
    public string NewScene;

    void Update()
    {
        transform.Rotate(new Vector3(0, Time.deltaTime * RotationSpeed, 0));
    }

    // When the player touches the pyramid, call function to load to the next scene.
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            ObtainCollectible(player);
        }
    }

    // Load to the next scene.
    private void ObtainCollectible(PlayerController player)
    {
        SceneManager.LoadScene(NewScene);
    }
}
