using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextManager : MonoBehaviour
{
    public static TextManager instance;

    [SerializeField]
    public TMP_Text slimeCountText;

    private void Awake()
    {
        instance = this;
    }
    //TODO: super hacky but ok
    // Start is called before the first frame update
    void Start()
    {
       // slimeCountText = GetComponent<TMP_Text>();

        if(Sora.Managers.GameManager.instance != null)
        {
            int totalReqSlime = Sora.Managers.GameManager.instance.GetRequiredSlimeCount();
            int currSlime = Sora.Managers.GameManager.instance.GetCurrentSlimeCount();

            slimeCountText.text = currSlime.ToString() + " / " + totalReqSlime.ToString();
        }
    }

    public void UpdateText(int currSlime, int totalReqSlime)
    {
        slimeCountText.text = currSlime.ToString() + " / " + totalReqSlime.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
