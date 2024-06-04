using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    public class DialogueHolder : MonoBehaviour
    {
        [Header("Text Box")]
        [SerializeField] private RectTransform TextBox;
        [Header("Position of the Dialogue Box")]
        
        [SerializeField] private Vector2 posLowerScreen;
        [SerializeField] private Vector2 posUpperScreen;
        [Header("Speed for Textbox moving")]
        [SerializeField] private float speed;

        private IEnumerator dialogueSeq;
        private bool dialogueFinished;
       


        private void OnEnable()
        {
            dialogueSeq = dialogueSequence();
            StartCoroutine(dialogueSeq);
        }

        private IEnumerator dialogueSequence()
        {
            if (!dialogueFinished)
            {
                for (int i = 2; i < transform.childCount-1; i++)
                {
                    Deactivate();
                    transform.GetChild(i).gameObject.SetActive(true);
                    var pos = transform.GetChild(i).GetComponent<DialogueLine>().positionOnScreen;
                    StartCoroutine(SetDimensions(pos ? posLowerScreen : posUpperScreen));
                    yield return new WaitUntil(() => transform.GetChild(i).GetComponent<DialogueLine>().finished);
                    Debug.Log("returned");
                }
            }
            else
            {
                    int index = transform.childCount - 1;
                    Deactivate();
                    transform.GetChild(index).gameObject.SetActive(true);
                    var pos = transform.GetChild(index).GetComponent<DialogueLine>().positionOnScreen;
                    StartCoroutine(SetDimensions(pos ? posLowerScreen : posUpperScreen));
                yield return new WaitUntil(() => transform.GetChild(index).GetComponent<DialogueLine>().finished);
                
            }
            dialogueFinished = true;
            gameObject.SetActive(false);
        }

        private void Deactivate()
        {
            for (int i = 2; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private IEnumerator SetDimensions(Vector2 targetPos)
        {
          
           while (true)
            {
                TextBox.anchoredPosition = Vector2.MoveTowards(TextBox.anchoredPosition, targetPos, speed*Time.deltaTime);

                float distance = Vector2.Distance(TextBox.anchoredPosition, targetPos);

                if(Mathf.Round(distance) == 0)
                {
                    yield break;
                }

                yield return null;

            }

        }

      
    }
}
