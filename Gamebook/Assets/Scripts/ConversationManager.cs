using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;


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
    public Image middleImage;

    public AudioSource bgmAudioSource;
    public AudioSource sfxAudioSource;

    public GameObject lifeCountImg;
    public Image[] lifeCases;
    

    private List<Dictionary<string, object>> dialogueData;
    private int currentID = 0;
    private int maxHP = 3;
    private int currentHP = 3;

    public int saveID;
    public int conID;
    public int saveHP;
    public int conHP;
    private Dictionary<int, GameSnapshot> snapshots = new Dictionary<int, GameSnapshot>();

    void Start()
    {
        LoadGame();
        LoadDialogueData();
        DisplayDialogue(currentID);
        conHP = currentHP;
        conID = currentID;
    }
    public void SaveGame()
    {
        snapshots[currentID] = new GameSnapshot(currentID, currentHP);
        PlayerPrefs.SetString("Snapshots", JsonUtility.ToJson(snapshots)); // 저장된 스냅샷을 PlayerPrefs에 문자열로 저장

        // 다른 변수들도 저장...
        PlayerPrefs.SetInt("CurrentID", currentID);
        PlayerPrefs.SetInt("CurrentHP", currentHP);

        PlayerPrefs.Save();
    }

    public void LoadGame()
    {


        if (PlayerPrefs.HasKey("CurrentID"))
        {
            currentID = PlayerPrefs.GetInt("CurrentID");
        }

        if (PlayerPrefs.HasKey("CurrentHP"))
        {
            currentHP = PlayerPrefs.GetInt("CurrentHP");
        }

        // 불러오기 시 저장된 스냅샷 복원
        if (PlayerPrefs.HasKey("Snapshots"))
        {
            snapshots = JsonUtility.FromJson<Dictionary<int, GameSnapshot>>(PlayerPrefs.GetString("Snapshots"));
        }
        // 다른 변수들 불러오기...
        // 추가적인 로직...
    }



    // 저장 버튼이 눌렸을 때 호출되는 메서드
    public void OnSaveButtonClicked()
    {
        SaveGame();

        Debug.Log(currentID);
        saveID = currentID;
        saveHP = currentHP;
        conHP = currentHP;
        conID = currentID;


    }

    // 불러오기 버튼이 눌렸을 때 호출되는 메서드
    public void OnLoadButtonClicked()
    {
        saveID = conID;
        saveHP = conHP;

        LoadGame();
        if (snapshots.ContainsKey(saveID))
        {
            RestoreSnapshot(snapshots[saveID]);
            RestoreSnapshot(snapshots[saveHP]);
        }

        DisplayDialogue(saveID);
        Debug.Log(saveID);
    }

    public void Continue()
    {
        LoadGame();
        if (snapshots.ContainsKey(conID))
        {
            RestoreSnapshot(snapshots[conID]);
            RestoreSnapshot(snapshots[conHP]);
        }
        DisplayDialogue(conID);
        Debug.Log(conID);

    }

    void RestoreSnapshot(GameSnapshot snapshot)
    {
        currentID = snapshot.CurrentID;
        currentHP = snapshot.CurrentHP;
        // 다른 변수들 복원...

        SaveGame(); // 복원된 상태를 다시 저장
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
                currentID= id;
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
                if (characterLeft.sprite == null)
                {
                    characterLeft.color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    if (LeftOpacity == 1)
                    {
                        characterLeft.color = new Color(0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        characterLeft.color = new Color(1f, 1f, 1f);
                    }
                }
                if (characterRight.sprite == null)
                {
                    characterRight.color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    if (RightOpacity == 1)
                    {
                        characterRight.color = new Color(0.5f, 0.5f, 0.5f);
                    }
                    else
                    {
                        characterRight.color = new Color(1f, 1f, 1f);
                    }
                }

                string bgmFileName = dialogueData[idx]["BGM"].ToString();
                if (bgmFileName != "-1")
                {
                    if (bgmFileName == "0")
                    {
                        // BGM 정지
                        bgmAudioSource.Stop();
                    }
                    else
                    {
                        // BGM 재생 (Resources/BGM 경로에 있는 파일을 재생)
                        AudioClip bgmClip = Resources.Load<AudioClip>("BGM/" + bgmFileName);

                        if (bgmClip != null)
                        {
                            bgmAudioSource.clip = bgmClip;
                            bgmAudioSource.loop = true; // BGM 반복 재생 설정
                            bgmAudioSource.Play();
                        }
                    }
                }


                string sfxFileName = dialogueData[idx]["SFX"].ToString();
                if (sfxFileName != "-1")
                {
                    if (sfxFileName == "0")
                    {
                        // SFX 정지
                        sfxAudioSource.Stop();
                    }
                    else
                    {
                        // SFX 재생 (Resources/SFX 경로에 있는 파일을 재생)
                        AudioClip sfxClip = Resources.Load<AudioClip>("SFX/" + sfxFileName);

                        if (sfxClip != null)
                        {
                            sfxAudioSource.clip = sfxClip;
                            sfxAudioSource.PlayOneShot(sfxClip);
                        }
                    }
                }

                string middleImageName = dialogueData[idx]["MiddleImage"].ToString();
                UpdateMiddleImage(middleImageName); // MiddleImage 업데이트 함수 호출

                FadeScript fadeScript = FindObjectOfType<FadeScript>();
                int effect = (int)dialogueData[idx]["Effect"];
                if(effect != -1)
                {
                    if(effect == 1)
                    {
                        fadeScript.Fade();
                    }
                    else if(effect == 2)
                    {
                        fadeScript.white();
                    }
                    else if(effect ==3)
                    {
                        fadeScript.red();
                        //red();
                    }
                    else if (effect == 4)
                    {
                        fadeScript.Green();
                        //green();
                    }
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

    void UpdateMiddleImage(string imageName)
    {
        if (imageName != "-1")
        {
            if (imageName == "0")
            {
                // 이미지 삭제
                middleImage.sprite = null; // 이미지 제거
                middleImage.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                // 새로운 이미지 표시 (Resources/Image 경로에 있는 파일을 불러와 표시)
                Sprite middleSprite = Resources.Load<Sprite>("Image/" + imageName);
                if (middleSprite != null)
                {
                    middleImage.sprite = middleSprite;
                    middleImage.color = new Color(1f, 1f, 1f, 1f);
                }
            }
        }


    }
    [System.Serializable]
    public class GameSnapshot
    {
        public int CurrentID;
        public int CurrentHP;

        public GameSnapshot(int currentID, int currentHP)
        {
            CurrentID = currentID;
            CurrentHP = currentHP;
        }
    }
}


