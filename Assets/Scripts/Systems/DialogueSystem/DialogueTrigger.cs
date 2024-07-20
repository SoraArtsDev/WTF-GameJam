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
using Sora.Events;

namespace Sora.DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private List<string> dialogues;
        [SerializeField] private float dialogueReplayCD;
        [SerializeField] private bool fireEventOnCompletion;

        [ShowIf("fireEventOnCompletion", true)]
        [SerializeField] private SoraEvent dialogueEndEvent;
        private bool next;
        private bool skip;
        private Coroutine dialogueCoroutine;

        [Space]
        [SerializeField] private GameObject dialogueCanvas;
        [SerializeField] private TMP_Text dialogueText;

        private bool visited;

        private void OnEnable()
        {
            // subscribe input event
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!visited)
            {
                visited = true;
                dialogueCoroutine = StartCoroutine(ShowDialogue());

                dialogueCanvas.SetActive(false);
                if (fireEventOnCompletion)
                    dialogueEndEvent.InvokeEvent();

                StartCoroutine(ResetDialogueCD());
            }
        }

        private IEnumerator ResetDialogueCD()
        {
            yield return new WaitForSecondsRealtime(dialogueReplayCD);
            visited = false;
        }

        private void Update()
        {
            if(skip)
            {
                StopCoroutine(dialogueCoroutine);
                skip = false;

            }
        }

        private IEnumerator ShowDialogue()
        {
            dialogueCanvas.SetActive(true);
            foreach(string dialogue in dialogues)
            {
                next = false;
                dialogueText.text = "";
                for(int i = 0; i < dialogue.Length; ++i)
                {
                    dialogueText.text += dialogue[i].ToString();

                    yield return new WaitForSecondsRealtime(0.03f);
                }

                yield return new WaitUntil(() => next == true);
            }

            dialogueCanvas.SetActive(false);
            if (fireEventOnCompletion)
                dialogueEndEvent.InvokeEvent();

            StartCoroutine(ResetDialogueCD());
        }

        void OnNext()
        {
            next = true;
        }

        void OnSkip()
        {
            skip = true;
        }
    }
}