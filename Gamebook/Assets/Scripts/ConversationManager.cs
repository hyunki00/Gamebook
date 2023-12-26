using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;


public class ConversationManager : MonoBehaviour
{
    public string dialogueFileName = "Dialogue.csv";
    public TextMeshProUGUI dialogueText;
    public GameObject nextButton;
    public GameObject choiceGroupImg;
    public GameObject[] choiceBoxes;
    public TextMeshProUGUI[] choiceTexts;
    public TextMeshProUGUI nameText;
    public Image characterLeft;
    public Image characterRight;
    private Tween shakeTween_L;
    private Tween shakeTween_R;
    public Image BackGround_img;
    public Image BackGround_img_shake;

    public GameObject lifeCountImg;
    public Image[] lifeCases;

    private List<Dictionary<string, object>> dialogueData;
    private int currentID = 0;
    private int maxHP = 3;
    private int currentHP = 3;


    void Start()
    {
        LoadDialogueData();
        DisplayDialogue(currentID);
    }

    void LoadDialogueData()
    {
        dialogueData = CSVReader.Read("DialgueTest01");
    }

    void DisplayDialogue(int id)
    {
        ClearChoiceButtons();

        for (int idx = 0; idx < dialogueData.Count; idx++)
        {
            int dialogID = (int)dialogueData[idx]["ID"];
            if (dialogID == id)
            {
                string characterName = dialogueData[idx]["Name"].ToString();
                nameText.text = characterName;

                string dialogContent = dialogueData[idx]["Dialog"].ToString();
                dialogueText.text = dialogContent;

                int isChoice = (int)dialogueData[idx]["IsChoice"];
                if (isChoice > 0)
                {
                    ActivateChoiceBoxes(isChoice);
                    SetChoiceButtonActions(id, isChoice);
                }
                else if ((int)dialogueData[idx]["ChoiceTo"] > 0)
                {
                    DeactivateChoiceBoxes();
                    ShowNextDialogue((int)dialogueData[idx]["ChoiceTo"]);
                }
                else
                {
                    DeactivateChoiceBoxes();
                    ShowNextDialogue(id + 1);
                }
                string imageName = dialogueData[idx]["Image_L"].ToString();
                string imageName_R = dialogueData[idx]["Image_R"].ToString();

                UpdateCharacterImage(imageName, imageName_R);
                UpdateHPUI(id);

                string bgImageName = dialogueData[idx]["BGImage"].ToString();
                UpdateBGImage(bgImageName);

                int isShake_L = (int)dialogueData[idx]["isShake_L"];

                if (isShake_L == 1)
                {
                    // 이전 tween을 멈춥니다.
                    if (shakeTween_L != null && shakeTween_L.IsActive())
                    {
                        shakeTween_L.Kill();
                    }
                    RectTransform rectTransform_L = characterLeft.gameObject.GetComponent<RectTransform>();
                    shakeTween_L = rectTransform_L.DOShakePosition(duration: 0.5f, strength: 100, vibrato: 100);
                }
                else
                {
                    // 이전 tween을 멈춥니다.
                    if (shakeTween_L != null && shakeTween_L.IsActive())
                    {
                        shakeTween_L.Kill();
                    }
                    characterLeft.gameObject.GetComponent<RectTransform>().DOShakePosition(0.00001f);
                }

                int isShake_R = (int)dialogueData[idx]["isShake_R"];
                if (isShake_R == 1)
                {
                    // 이전 tween을 멈춥니다.
                    if (shakeTween_R != null && shakeTween_R.IsActive())
                    {
                        shakeTween_R.Kill();
                    }
                    RectTransform rectTransform_R = characterRight.gameObject.GetComponent<RectTransform>();
                    shakeTween_R = rectTransform_R.DOShakePosition(duration: 0.5f, strength: 100, vibrato: 100);
                }
                else
                {
                    // 이전 tween을 멈춥니다.
                    if (shakeTween_R != null && shakeTween_R.IsActive())
                    {
                        shakeTween_R.Kill();
                    }
                    characterRight.gameObject.GetComponent<RectTransform>().DOShakePosition(0.00001f);
                }

                int isCameraShake = (int)dialogueData[idx]["isCameraShake"];
                if (isCameraShake == 1)
                {
                    if (BackGround_img != null)
                    {
                        BackGround_img.gameObject.GetComponent<RectTransform>().DOShakePosition(duration: 0.5f, strength: 100, vibrato: 100);
                    }
                }
                int LeftOpacity = (int)dialogueData[idx]["LeftOpacity"];
                int RightOpacity = (int)dialogueData[idx]["RightOpacity"];
                if (LeftOpacity == 1)
                {
                characterRight.color = new Color(0.5f, 0.5f, 0.5f);

                }
                else
                {
                characterRight.color = new Color(1f, 1f, 1f);
                }
                if( RightOpacity == 1)
                {
                characterLeft.color = new Color(0.5f, 0.5f, 0.5f);
                }
                else
                {
                characterLeft.color = new Color(1f, 1f, 1f);
                }
            }
        }
    }

    void SetChoiceButtonActions(int id, int numOfChoices)
    {
        int nextId = id + 1;

        for (int i = 0; i < numOfChoices; i++)
        {
            if (nextId < dialogueData.Count)
            {
                int choiceTo = (int)dialogueData[nextId]["ChoiceTo"];
                int choiceID = nextId; // Store the choice ID

                choiceTexts[i].text = dialogueData[nextId]["Dialog"].ToString();

                UnityEngine.UI.Button buttonComponent = choiceBoxes[i].GetComponent<UnityEngine.UI.Button>();
                buttonComponent.onClick.RemoveAllListeners();
                buttonComponent.onClick.AddListener(() => OnChoiceButtonClick(choiceTo, choiceID)); // Pass both choiceTo and choiceID

                nextId++;
            }
        }
    }


    void OnChoiceButtonClick(int choiceTo, int choiceID)
    {
        DisplayDialogue(choiceTo);
    }

    void ShowNextDialogue(int nextID)
    {
        nextButton.SetActive(true);
        nextButton.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        nextButton.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => DisplayDialogue(nextID));
    }

    void ClearChoiceButtons()
    {
        choiceGroupImg.SetActive(false);
    }

    void DeactivateChoiceBoxes()
    {
        choiceGroupImg.SetActive(false);
    }

    void ActivateChoiceBoxes(int numOfChoices)
    {
        choiceGroupImg.SetActive(true);
        for (int i = 0; i < numOfChoices; i++)
        {
            choiceBoxes[i].SetActive(true);
        }
    }

    void UpdateCharacterImage(string imageName, string imageName_R)
    {
        if (imageName != "-1")
        {
            if (imageName == "0")
            {
                characterLeft.sprite = null; // 이미지 제거
                characterLeft.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                Sprite characterSprite = Resources.Load<Sprite>("Image/" + imageName);
                if (characterSprite != null)
                {
                    characterLeft.sprite = characterSprite;
                    characterLeft.color = new Color(1f, 1f, 1f, 1f); // 이미지가 표시되어야 하므로 완전히 불투명하게 설정

                }
            }
        }

        if (imageName_R != "-1")
        {
            if (imageName_R == "0")
            {
                characterRight.sprite = null; // 이미지 제거
                characterRight.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                Sprite characterSprite_R = Resources.Load<Sprite>("Image/" + imageName_R);
                if (characterSprite_R != null)
                {
                    characterRight.sprite = characterSprite_R;
                    characterRight.color = new Color(1f, 1f, 1f, 1f); // 이미지가 표시되어야 하므로 완전히 불투명하게 설정

                }

            }
        }
    }

    void UpdateBGImage(string imageName)
    {
        if (imageName != "-1")
        {
            if (imageName == "0")
            {
                BackGround_img.sprite = null; // 이미지 제거
                BackGround_img.color = new Color(1f, 1f, 1f, 0f);

                BackGround_img_shake.sprite = null; // 이미지 제거
                BackGround_img_shake.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                Sprite BGSprite = Resources.Load<Sprite>("BG/" + imageName);
                if (BGSprite != null)
                {
                    BackGround_img.sprite = BGSprite;
                    BackGround_img.color = new Color(1f, 1f, 1f, 1f); // 이미지가 표시되어야 하므로 완전히 불투명하게 설정
                    BackGround_img_shake.sprite = BGSprite;
                    BackGround_img_shake.color = new Color(1f, 1f, 1f, 1f); // 이미지가 표시되어야 하므로 완전히 불투명하게 설정
                }
            }
        }
    }

        void UpdateHPUI(int id)
    {
        int hpValue = (int)dialogueData[id]["HP"];
        currentHP += hpValue;
        Debug.Log("현재체력:" + currentHP + " 깎인 체력:" + hpValue);
        for (int i = 0; i < maxHP; i++)
        {
            if (i < currentHP)
            {
                lifeCases[i].gameObject.SetActive(true);
            }
            else
            {
                lifeCases[i].gameObject.SetActive(false);
            }
        }

        if (currentHP <= 0)
        {
            // 게임 오버 로직 추가
            Debug.Log("게임 오버");
        }
    }
}


