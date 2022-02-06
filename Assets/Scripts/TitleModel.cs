using UnityEngine;
using UnityEngine.SceneManagement;

// This Component represents the title screen. It controls the player model that shows on the title screen, and also loads the first scene when any key is pressed. 
public class TitleModel : MonoBehaviour
{
    [Tooltip("Speed to rotate the player on the title screen.")]
    public float RotateAmount = 15;

    [Tooltip("Starting scene to load into after key press.")]
    public string NewSceneName = "Level0";

    // Randomize color of fake player on title screen.
    void Start()
    {
        var meshes = GetComponentsInChildren<MeshRenderer>();
        foreach(var mesh in meshes)
        {
            var color = new Color(Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f), Random.Range(0.2f, 1.0f));
            mesh.materials[0].color = color;
        }
    }

    // If player presses any key, load the next scene.
    void Update()
    {
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Sin(Time.timeSinceLevelLoad) * RotateAmount);

        if(Input.anyKeyDown)
        {
            SceneManager.LoadScene(NewSceneName);
        }
    }
}
