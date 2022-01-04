using UnityEngine;
using UnityEngine.UI;

public class ExitUI : MonoBehaviour
{
    [Header("UI References :")]
    [SerializeField] private Button exitButton;
    // Start is called before the first frame update
    void Start()
    {
        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    // Update is called once per frame
    private void OnDestroy()
    {
        
    }
}
