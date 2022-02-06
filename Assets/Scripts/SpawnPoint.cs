using UnityEngine;

// This component represents a spawn point that a player can touch to reset where they will spawn next time they reincarnate.
public class SpawnPoint : MonoBehaviour
{
    // When the player touches this, reset their timer, and set this as the new spawn point.
    private void OnTriggerEnter(Collider other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if(player != null)
        {
            player.SpawnPoint = gameObject;
            player.isTimerPaused = true;
            player.timer = player.RespawnTime;
        }
    }

    // Don't tick down the timer as long as the player is touching this.
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.isTimerPaused = true;
                player.timer = player.RespawnTime;
            }
        }
    }

    // When the player leaves, resume timer count down.
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            var player = other.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.isTimerPaused = false;
            }
        }
    }
}
