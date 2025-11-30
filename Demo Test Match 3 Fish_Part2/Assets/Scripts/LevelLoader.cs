using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    [Range(0.1f, 1f)] public float transitionTime = 0.5f; // Allow adjusting in the inspector with a slider
    public bool instantTransition = false; // Toggle for instant transition

#if UNITY_EDITOR
    [SerializeField]
    private Button casualGameButton;

    [SerializeField]
    private Button backButton;
#endif

    void Start()
    {
      
    }

   
}
