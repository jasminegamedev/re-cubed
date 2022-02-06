using UnityEngine;
using UnityEngine.Events;

// This component represents a button. When the player or a player corpse is touching the button, it will remain active, but when they leave, it will deactivate.
public class ButtonSwitch : MonoBehaviour
{
    [Tooltip("Event that gets called when the button is activated.")]
    public UnityEvent OnActivate;
    [Tooltip("Event that gets called when the button is deactivated.")]
    public UnityEvent OnDeactivate;

    // Whether the button is currently Active.
    private bool _active;

    // When a player or corpse starts touching this trigger, call the activate event.
    private void OnTriggerEnter(Collider other)
    {
        if (!_active)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Corpse"))
            {
                _active = true;
                OnActivate.Invoke();
            }
        }
    }

    // When a player or corpse touches this trigger when it wasn't already active, call the activate event.
    private void OnTriggerStay(Collider other)
    {
        if (!_active)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Corpse"))
            {
                _active = true;
                OnActivate.Invoke();
            }
        }
    }

    // When a player or corpse leaves this trigger when it was active, call the deactivate event.
    private void OnTriggerExit(Collider other)
    {
        if (_active)
        {
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Corpse"))
            {
                _active = false;
                OnDeactivate.Invoke();
            }
        }
    }
}
