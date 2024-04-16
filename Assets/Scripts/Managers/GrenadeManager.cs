using UnityEngine;
using UnityEngine.UI;

public class GrenadeManager : MonoBehaviour
{
    public static int grenades;                             // The player's score.
    [SerializeField] private Text grenadeText;                    // Reference to the Text component.
    
    void Awake()
    {
        // Set up the reference.
        if (grenadeText == null)
            grenadeText = GetComponent<Text>();

        // Reset the score.
        grenades = 0;
    }
    
    void Update()
    {
        // Set the displayed text to be the word "Score" followed by the score value.
        grenadeText.text = "Grenades: " + grenades;
    }
}