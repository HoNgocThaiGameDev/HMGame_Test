using UnityEngine;

public class AdjustUiLayout : MonoBehaviour
{
    public RectTransform centerLayout;    

    private int previousWidth;
    private int previousHeight;

    void Start()
    {
        AdjustUILayout();
    }

    void AdjustUILayout()
    {
        if (centerLayout == null)
        {
            Debug.LogError("eeferences are not assigned");
            return;
        }

        float aspectRatio = (float)Screen.width / Screen.height;

        if (aspectRatio >= 1.7f) 
        {
            centerLayout.localScale = new Vector3(1.1f, 1.1f, 1);
        }
        else 
        {
            centerLayout.localScale = new Vector3(1.6f, 1.6f, 1);  
        }
    }

    void Update()
    {
        if (Screen.width != previousWidth || Screen.height != previousHeight)
        {
            previousWidth = Screen.width;
            previousHeight = Screen.height;
            AdjustUILayout();
        }
    }
}
