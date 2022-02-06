using UnityEngine;

// Component that controls panning between two points when it has been activated.
public class Panner : MonoBehaviour
{
    [Tooltip("Offset that we want to pan to.")]
    public Vector3 offset;
    [Tooltip("How fast we want to pan.")]
    public float MoveSpeed = 10;
    [Tooltip("How many things need to be called to activate this.")]
    public int ActiveCost = 1;

    // Delta of how far we have panned so far.
    private float _delta;
    // How many things have tried to activate this.
    private int _active;
    // Stored starting position.
    private Vector3 _startPosition;

    void Start()
    {
        _startPosition = transform.localPosition;
    }

    void Update()
    {
        if (_active >= ActiveCost)
        {
            _delta += MoveSpeed * Time.deltaTime;
        }
        else
        {
            _delta -= MoveSpeed * Time.deltaTime;
        }
        _delta = Mathf.Clamp01(_delta);
        transform.localPosition = Vector3.Lerp(_startPosition, _startPosition + offset, _delta);
    }

    public void Activate()
    {
        _active ++;
    }

    public void Deactivate()
    {
        _active --;
    }
}
