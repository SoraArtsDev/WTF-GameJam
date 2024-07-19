// Developed by Pluto
//
// Copyright(c) Sora Arts 2023-2024
//
// This script is covered by a Non-Disclosure Agreement (NDA) and is Confidential.
// Destroy the file immediately if you have not been explicitly granted access.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Sora.DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private List<string> dialogues;
        [SerializeField] private float dialogueReplayCD;

        [Space]
        [SerializeField] private GameObject dialogueCanvas;
        [SerializeField] private TMP_Text dialogueText;

        private bool visited;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!visited)
            {
                visited = true;
                StartCoroutine(ShowDialogue());
            }
        }

        private IEnumerator ResetDialogueCD()
        {
            yield return new WaitForSecondsRealtime(dialogueReplayCD);
            visited = false;
        }

        private IEnumerator ShowDialogue()
        {
            dialogueCanvas.SetActive(true);
            foreach(string dialogue in dialogues)
            {
                dialogueText.text = "";
                for(int i = 0; i < dialogue.Length; ++i)
                {
                    dialogueText.text += dialogue[i].ToString();

                    yield return null;
                }

                dialogueText.text += "->";
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            }

            dialogueCanvas.SetActive(false);
            StartCoroutine(ResetDialogueCD());
        }
    }
}