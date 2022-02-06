using UnityEngine;

// Represents a repeating floor hazard (Lava or Spikes)
public class Hazard : MonoBehaviour
{
    [Tooltip("How many times this object should repeat on X Axis.")]
    public int Width = 1;
    [Tooltip("How many times this object should repeat on Z Axis.")]
    public int Height = 1;
    
    [Tooltip("Whether we want to scroll the texture or not.")]
    public bool ScrollTexture = false;
    [Tooltip("How fast we should scroll the texture.")]
    public Vector2 uvAnimationRate = new Vector2(1.0f, 1.0f);
    [Tooltip("Name of the texture we want to scroll.")]
    public string textureName = "_MainTex";

    // How far the texture has scrolled.
    private Vector2 uvOffset = Vector2.zero;
    // Cache variable for the main material on the model.
    private Material _material;

    // Spawn grid of child objects based on width and height.
    void Start()
    {
        GameObject childObject = transform.GetChild(0).gameObject;

        _material = childObject.GetComponent<MeshRenderer>().sharedMaterials[0];
        BoxCollider collider = GetComponent<BoxCollider>();
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                var newObject = Instantiate(childObject, transform);
                newObject.transform.position = new Vector3(transform.position.x + transform.localScale.x * (-Width / 2.0f + 0.5f + i), childObject.transform.position.y, transform.position.z + transform.localScale.z * (-Height / 2.0f + 0.5f + j));
            }
        }
        collider.size = new Vector3(collider.size.x * Width, collider.size.y, collider.size.z * Height);
        Destroy(childObject);
    }

    // Scroll main texture if ScrollTexture is true.
    void Update()
    {
        if (ScrollTexture)
        {
            uvOffset += (new Vector2(Mathf.Sin(uvAnimationRate.x * Time.deltaTime), Mathf.Sin(uvAnimationRate.y * Time.deltaTime)));
            _material.SetTextureOffset(textureName, uvOffset);
        }
    }

    // Kill the player if they touch this.
    private void OnCollisionEnter(Collision collision)
    {
        var player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.FakeDie();
        }
    }
}
