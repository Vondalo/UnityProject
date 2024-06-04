using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueLine : DialogueBaseClass
    {
        [Header("Text Optionen")]
        [SerializeField]
        private string input;
        private TMP_Text textHolder;
        [SerializeField] private Color textColor;
        [SerializeField] private TMP_FontAsset textFont;


        [Header("Delay Optionen")]
        [SerializeField] private float delay;
        [SerializeField] private float delayBetweenLines;
        

        [Header("Audio Optionen")]
        [SerializeField] private AudioClip sound;

        [Header("Delay Optionen")]
        [SerializeField] private Sprite? characterSprite;
        [SerializeField] private Image imageHolder;

       
        [Header("Check Box if at Bottom, else at top")]
        [SerializeField] public bool positionOnScreen;

        private IEnumerator lineAppear;

        private void Awake()
        {
            textHolder = GetComponent<TMP_Text>();
            textHolder.text = "";
            
            
            imageHolder.sprite = characterSprite;
            imageHolder.preserveAspect = true;
           

        }
        private void OnEnable()
        {
            ResetLine();
            lineAppear = (WriteText(input, textHolder, textColor, textFont, delay, sound,delayBetweenLines));
            StartCoroutine(lineAppear);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.X))
            {
                if(textHolder.text != input)
                {
                    StopCoroutine(lineAppear);
                   
                  
                    
                    textHolder.text = input;

                    // Warten bis der Spieler Space drückt weil ja die Coroutine davor beendet wurde und das nicht mehr machen kann
                    StartCoroutine(WaitForSpace());

                }
                else
                {
                    finished = true;
                }
            }
        }


        private void ResetLine()
        {
            textHolder = GetComponent<TMP_Text>();
            textHolder.text = "";
            finished = false;
        }
        }
    }



