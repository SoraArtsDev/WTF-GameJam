using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public enum ECameraControl
{
    ENONE,
    EPAN_IN,
    EPAN_OUT,
};

public class CamerControls : MonoBehaviour
{
    public ECameraControl controlType;
    public float duration;
    public CharacterController2D playerController;
    public CinemachineVirtualCamera virtualCamera;
    public CinemachineFramingTransposer framingTransposer;
    public bool oneShot;
    public bool isEnabled;

    private float initialOrthographicSize;
    public  float targetOrthographicSize;
    public  float elapsedTime;
    public  float screenStartX;
    public  float screenStartY;
    public  float screenX;
    public  float screenY;
    private bool isFlipped;

    public bool canLerpScreen;
    public bool canLerpSize;



    private void Awake()
    {
        playerController = GameObject.Find("player").GetComponent<CharacterController2D>();
        virtualCamera = GameObject.Find("virtualCamera").GetComponent<CinemachineVirtualCamera>();
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
       

        initialOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
        if (controlType == ECameraControl.EPAN_IN)
        {
            targetOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
        }

        elapsedTime = 0.0f;
    }

    private void Start()
    {
        if (virtualCamera.GetComponent<CinemachinePixelPerfect>())
        {
            virtualCamera.GetComponent<CinemachinePixelPerfect>().enabled = false;
        }
    }

    private void Update()
    {
        if (isEnabled)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            if(canLerpSize)
            {
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(initialOrthographicSize, targetOrthographicSize, t);
            }

            if (canLerpSize)
            {
                if (isFlipped)
                {
                    t = targetOrthographicSize / virtualCamera.m_Lens.OrthographicSize;
                }
                else
                {
                    t = virtualCamera.m_Lens.OrthographicSize / targetOrthographicSize;
                }

            }

            if (canLerpScreen)
            {
                framingTransposer.m_ScreenX  = Mathf.Lerp(screenStartX, screenX, t);
                framingTransposer.m_ScreenY  = Mathf.Lerp(screenStartY, screenY, t);
            }

            if (Mathf.Approximately(virtualCamera.m_Lens.OrthographicSize, targetOrthographicSize))
            {
                isEnabled = false;
                virtualCamera.m_Lens.OrthographicSize = targetOrthographicSize;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            initialOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
            isFlipped = initialOrthographicSize > targetOrthographicSize;
            screenStartX = framingTransposer.m_ScreenX;
            screenStartY = framingTransposer.m_ScreenY;
            isEnabled = true;
            elapsedTime = 0.0f;
        }
    }
}
