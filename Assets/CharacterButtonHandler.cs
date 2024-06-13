using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterButtonHandler : MonoBehaviour
{
    [SerializeField]private List<Button> characterButtons = new List<Button>();
    [SerializeField] public List<Character> Prefabs = new List<Character>();
    [SerializeField] private GameObject[] takenUI = null;
    [SerializeField] public JoinMenu joinMenu = null;
    [SerializeField] GameObject MainMenu = null;
    public Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
        foreach (Button button in characterButtons)
        {
            button.onClick.AddListener(() => joinMenu.SetPlayerGameObject(characterButtons.IndexOf(button)));
            button.onClick.AddListener(() => SetButtonTaken(characterButtons.IndexOf(button)));
        }
    }

    private void SetButtonTaken(int index)
    {
        foreach(Button button in characterButtons)
        {
            takenUI[characterButtons.IndexOf(button)].SetActive(false);
        }
        takenUI[index].SetActive(true);
    }

    private void Awake()
    {
        SetButtonTaken(PlayerPrefs.GetInt("prefabIndex",0));
    }

    public void InitiateAnimationReturn()
    {
        
        animator.SetBool("Return", true);
        MainMenu.SetActive(true);
    }
    public void DisableMenu()
    {
        animator.SetBool("Return", false);
       
        gameObject.SetActive(false);
    }

}

