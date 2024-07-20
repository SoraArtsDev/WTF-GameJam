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
    public bool oneShot;
    public bool isEnabled;

    private float initialOrthographicSize;
    public  float targetOrthographicSize;
    public  float elapsedTime;

    private void Awake()
    {
        playerController = GameObject.Find("player").GetComponent<CharacterController2D>();
        virtualCamera = GameObject.Find("virtualCamera").GetComponent<CinemachineVirtualCamera>();

        initialOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
        if (controlType == ECameraControl.EPAN_IN)
        {
            targetOrthographicSize = virtualCamera.m_Lens.OrthographicSize;
        }

        elapsedTime = 0.0f;
    }

    private void Update()
    {
        if (isEnabled)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(initialOrthographicSize, targetOrthographicSize, t);


            if(Mathf.Approximately(virtualCamera.m_Lens.OrthographicSize, targetOrthographicSize))
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
            isEnabled = true;
            elapsedTime = 0.0f;
        }
    }
}
