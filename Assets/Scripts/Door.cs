using UnityEngine;
using UnityEngine.Events;

// This component represents a door object. When a player collides with a door, and has a key, it will activate the door, presumable to pan the door down or destroy it.
public class Door : MonoBehaviour
{
    // Function that gets called when player collides with the door with a key.
    public UnityEvent OnActivate;

    // When the player collides with the door, if they have a key, decrement their key count, and call on activate event.
    private void OnCollisionEnter(Collision collision)
    {
        if (enabled && collision.gameObject.tag == "Player")
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null && player.keys > 0)
            {
                player.keys--;
                OnActivate.Invoke();
                enabled = false;
            }
        }
    }
}
