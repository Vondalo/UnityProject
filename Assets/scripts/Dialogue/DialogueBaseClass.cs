using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem 
{
    public class DialogueBaseClass : MonoBehaviour
    {
        public bool finished {  get; protected set;}
        protected IEnumerator WriteText(string input , TMP_Text textholder, Color textColor, TMP_FontAsset textFont, float delay,AudioClip sound, float delayBetweenLines)
        {
            textholder.color = textColor;
            textholder.font = textFont;
            if(textholder.text != "")
            {
                textholder.text = "";
            }
            

            for (int i = 0; i < input.Length; i++)
            {
                  textholder.text += input[i];

                if (input[i] != ' ')
                {
                    SoundManager.instance.PlaySound(sound);
                }
                  
                yield return new WaitForSeconds(delay);
            }
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            finished = true;
        }

        protected IEnumerator WaitForSpace()
        {
              yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
            finished = true;
        }
    }
}
