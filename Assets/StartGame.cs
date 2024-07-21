using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGame : MonoBehaviour
{
    public GameObject firstDialogue;
    private void OnEnable()
    {
        StartCoroutine(EnableDialogue());
    }

    private IEnumerator EnableDialogue()
    {
        yield return new WaitForSecondsRealtime(4.0f);
        firstDialogue.SetActive(true);
    }
}
