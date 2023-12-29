using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor.Experimental.GraphView;
using UnityEngine.SceneManagement;
using Febucci.UI.Core;
using Febucci.UI;
using UnityEngine.EventSystems;





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
    public TypewriterByCharacter Skipwriter;
    public Image NameBox;
    public GameObject lifeCountImg;
    public Image[] lifeCases;
    public Image GameOver;
    public bool isChoiceBtnClicked = false;
    public CanvasGroup ChaArea_Canvas;
    public Image TextBox_img;
    public Sprite endImage;
    public Button load_btn;
    public Button save_btn;


    private List<Dictionary<string, object>> dialogueData;
    private int currentID = 0;
    private int maxHP = 3;
    private int currentHP = 3;
    private string lastImageName;
    private string savedImageName;
    private string lastMiddleImageName;
    private string savedMiddleImageName;
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
        GameOver.gameObject.SetActive(false);

    }
    // SaveGame �޼��� ����
    // SaveGame �޼��� ����
    // SaveGame �޼��� ����
    // SaveGame �޼��� ����
    public void SaveGame()
    {
        snapshots[currentID] = new GameSnapshot(currentID, currentHP);

        // ���� ��ȭ ID�� �ش��ϴ� ������ ��������
        var currentDialogueData = dialogueData.Find(d => (int)d["ID"] == currentID);


        if (!string.IsNullOrEmpty(lastImageName))
        {
            savedImageName = lastImageName;
            PlayerPrefs.SetString("SavedImageName", savedImageName);
        }

        if (!string.IsNullOrEmpty(lastMiddleImageName))
        {
            savedMiddleImageName = lastMiddleImageName;
            PlayerPrefs.SetString("SavedMiddleImageName", savedMiddleImageName);
        }

        // ���� ��ȭ������ ��� �̹��� �̸� ��������
        lastImageName = currentDialogueData["BGImage"].ToString();
        // ���� ��ȭ������ Middle �̹��� �̸� ��������
        lastMiddleImageName = currentDialogueData["MiddleImage"].ToString();
        // �̹��� ���ϸ��� -1�� �ƴ� ��� PlayerPrefs�� ����
        if (lastMiddleImageName != "-1")
    {
        PlayerPrefs.SetString("savedMiddleImageName", savedMiddleImageName);
    }


        // �̹��� ���ϸ��� -1�� �ƴ� ��� PlayerPrefs�� ����
        if (lastImageName != "-1")
        {
            PlayerPrefs.SetString("LastImageName", lastImageName);
        }

        Debug.Log("Game Saved - ID: " + currentID + ", HP: " + currentHP + ", Last Image: " + lastImageName);

        PlayerPrefs.SetString("Snapshots", JsonUtility.ToJson(snapshots));
        PlayerPrefs.SetInt("CurrentID", currentID);
        PlayerPrefs.SetInt("CurrentHP", currentHP);
        PlayerPrefs.Save();
    }






    public void LoadGame()
    {
        savedImageName = PlayerPrefs.GetString("SavedImageName", "");
        savedMiddleImageName = PlayerPrefs.GetString("SavedMiddleImageName", "");

        if (PlayerPrefs.HasKey("CurrentID"))
        {
            currentID = PlayerPrefs.GetInt("CurrentID");
        }

        if (PlayerPrefs.HasKey("CurrentHP"))
        {
            currentHP = PlayerPrefs.GetInt("CurrentHP");
        }

        // �ҷ����� �� ����� ������ ����
        if (PlayerPrefs.HasKey("Snapshots"))
        {
            snapshots = JsonUtility.FromJson<Dictionary<int, GameSnapshot>>(PlayerPrefs.GetString("Snapshots"));
        }

        // ������ ǥ�õ� �̹��� �̸� �ҷ�����
        string lastImageName = PlayerPrefs.GetString("LastImageName", "");
        lastMiddleImageName = PlayerPrefs.GetString("LastMiddleImageName", "");

        // �̹��� �����̸��� ���� ��쿡�� ������Ʈ
        if (!string.IsNullOrEmpty(savedImageName))
        {
            UpdateBGImage(savedImageName);
        }
        if (!string.IsNullOrEmpty(savedMiddleImageName))
        {
            UpdateMiddleImage(savedMiddleImageName);
        }


        // �ٸ� ������ �ҷ�����...
        // �߰����� ����...
    }

    // ���� ��ư�� ������ �� ȣ��Ǵ� �޼���
    public void OnSaveButtonClicked()
    {
        SaveGame();

        Debug.Log(currentID);
        saveID = currentID;
        saveHP = currentHP;
        conHP = currentHP;
        conID = currentID;


    }

    // �ҷ����� ��ư�� ������ �� ȣ��Ǵ� �޼���
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
        GameOver.gameObject.SetActive(false);
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
        // �ٸ� ������ ����...

        SaveGame(); // ������ ���¸� �ٽ� ����
    }

    void LoadDialogueData()
    {
        dialogueData = CSVReader.Read("Dialogue");
    }

    void DisplayDialogue(int id)
    {
        ClearChoiceButtons();

        for (int idx = 0; idx < dialogueData.Count; idx++)
        {

            int dialogID = (int)dialogueData[idx]["ID"];
            if (dialogID == id)
            {
                Debug.Log(id);
                if (Skipwriter.isShowingText && isChoiceBtnClicked == false)
                {
                    Skipwriter.SkipTypewriter();
                    return;

                }
                isChoiceBtnClicked = false;

                currentID = id;

                int end = (int)dialogueData[idx]["End"];
                if (end > 0)
                {
                    string enddialogContent = dialogueData[idx]["Dialog"].ToString();
                    dialogueText.text = enddialogContent;

                    endImage = Resources.Load<Sprite>("BG/" + dialogueData[idx]["BGImage"].ToString());
                    BackGround_img.DOColor(Color.clear, 3f).OnComplete(()=> ChangeImage());
                    BackGround_img_shake.DOColor(Color.clear, 3f).OnComplete(() => ChangeImage());

                    ChaArea_Canvas.alpha = 0;
                    TextBox_img.gameObject.SetActive(false);
                    save_btn.gameObject.SetActive(false);
                    load_btn.gameObject.SetActive(false);
                    lifeCountImg.gameObject.SetActive(false);
                    nextButton.gameObject.SetActive(false);

                    return;

                }

                string characterName = dialogueData[idx]["Name"].ToString();
                if (characterName == "")
                {

                    NameBox.color = new Color(1f, 1f, 1f, 0f);
                    nameText.text = characterName;
                }
                else
                {
                    NameBox.color = new Color(1f, 1f, 1f, 1f);
                    nameText.text = characterName;
                }

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
                if (bgImageName != "-1")
                {
                    lastImageName = bgImageName;
                }
                Debug.Log("Last Image Name: " + lastImageName);

                int isShake_L = (int)dialogueData[idx]["isShake_L"];

                if (isShake_L == 1)
                {
                    // ���� tween�� ����ϴ�.
                    if (shakeTween_L != null && shakeTween_L.IsActive())
                    {
                        shakeTween_L.Kill();
                    }
                    RectTransform rectTransform_L = characterLeft.gameObject.GetComponent<RectTransform>();
                    shakeTween_L = rectTransform_L.DOShakePosition(duration: 0.5f, strength: 100, vibrato: 100);
                }
                else
                {
                    // ���� tween�� ����ϴ�.
                    if (shakeTween_L != null && shakeTween_L.IsActive())
                    {
                        shakeTween_L.Kill();
                    }
                    characterLeft.gameObject.GetComponent<RectTransform>().DOShakePosition(0.00001f);
                }

                int isShake_R = (int)dialogueData[idx]["isShake_R"];
                if (isShake_R == 1)
                {
                    // ���� tween�� ����ϴ�.
                    if (shakeTween_R != null && shakeTween_R.IsActive())
                    {
                        shakeTween_R.Kill();
                    }
                    RectTransform rectTransform_R = characterRight.gameObject.GetComponent<RectTransform>();
                    shakeTween_R = rectTransform_R.DOShakePosition(duration: 0.5f, strength: 100, vibrato: 100);
                }
                else
                {
                    // ���� tween�� ����ϴ�.
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
                        // BGM ����
                        bgmAudioSource.Stop();
                    }
                    else
                    {
                        // BGM ��� (Resources/BGM ��ο� �ִ� ������ ���)
                        AudioClip bgmClip = Resources.Load<AudioClip>("BGM/" + bgmFileName);

                        if (bgmClip != null)
                        {
                            bgmAudioSource.clip = bgmClip;
                            bgmAudioSource.loop = true; // BGM �ݺ� ��� ����
                            bgmAudioSource.Play();
                        }
                    }
                }


                string sfxFileName = dialogueData[idx]["SFX"].ToString();
                if (sfxFileName != "-1")
                {
                    if (sfxFileName == "0")
                    {
                        // SFX ����
                        sfxAudioSource.Stop();
                    }
                    else
                    {
                        // SFX ��� (Resources/SFX ��ο� �ִ� ������ ���)
                        AudioClip sfxClip = Resources.Load<AudioClip>("SFX/" + sfxFileName);

                        if (sfxClip != null)
                        {
                            sfxAudioSource.clip = sfxClip;
                            sfxAudioSource.PlayOneShot(sfxClip);
                        }
                    }
                }

                string middleImageName = dialogueData[idx]["MiddleImage"].ToString();
                UpdateMiddleImage(middleImageName); // MiddleImage ������Ʈ �Լ� ȣ��
                if (middleImageName != "-1")
                {
                    lastMiddleImageName = middleImageName;
                }

                FadeScript fadeScript = FindObjectOfType<FadeScript>();
                int effect = (int)dialogueData[idx]["Effect"];
                if (effect != -1)
                {
                    if (effect == 1)
                    {
                        fadeScript.Fade();
                    }
                    else if (effect == 2)
                    {
                        fadeScript.white();
                    }
                    else if (effect == 3)
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
        isChoiceBtnClicked = true;
        Debug.Log(choiceTo);
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
        for (int i = 0; i < 3; i++)
        {
            choiceBoxes[i].SetActive(false);
        }
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
                characterLeft.sprite = null; // �̹��� ����
                characterLeft.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                Sprite characterSprite = Resources.Load<Sprite>("Image/" + imageName);
                if (characterSprite != null)
                {
                    characterLeft.sprite = characterSprite;
                    characterLeft.color = new Color(1f, 1f, 1f, 1f); // �̹����� ǥ�õǾ�� �ϹǷ� ������ �������ϰ� ����

                }
            }
        }

        if (imageName_R != "-1")
        {
            if (imageName_R == "0")
            {
                characterRight.sprite = null; // �̹��� ����
                characterRight.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                Sprite characterSprite_R = Resources.Load<Sprite>("Image/" + imageName_R);
                if (characterSprite_R != null)
                {
                    characterRight.sprite = characterSprite_R;
                    characterRight.color = new Color(1f, 1f, 1f, 1f); // �̹����� ǥ�õǾ�� �ϹǷ� ������ �������ϰ� ����

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
                BackGround_img.sprite = null; // �̹��� ����
                BackGround_img.color = new Color(1f, 1f, 1f, 0f);

                BackGround_img_shake.sprite = null; // �̹��� ����
                BackGround_img_shake.color = new Color(1f, 1f, 1f, 0f);
            }
            else
            {
                Sprite BGSprite = Resources.Load<Sprite>("BG/" + imageName);
                if (BGSprite != null)
                {
                    BackGround_img.sprite = BGSprite;
                    BackGround_img.color = new Color(1f, 1f, 1f, 1f); // �̹����� ǥ�õǾ�� �ϹǷ� ������ �������ϰ� ����
                    BackGround_img_shake.sprite = BGSprite;
                    BackGround_img_shake.color = new Color(1f, 1f, 1f, 1f); // �̹����� ǥ�õǾ�� �ϹǷ� ������ �������ϰ� ����
                }
            }
        }
    }

    void UpdateHPUI(int id)
    {
        int hpValue = (int)dialogueData[id]["HP"];
        currentHP += hpValue;

        if (currentHP > 3)
        {
            currentHP = 3;
        }

        Debug.Log("����ü��:" + currentHP + " ���� ü��:" + hpValue);

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
    }

        void UpdateMiddleImage(string imageName)
        {
            if (imageName != "-1")
            {
                if (imageName == "0")
                {
                    // �̹��� ����
                    middleImage.sprite = null; // �̹��� ����
                    middleImage.color = new Color(1f, 1f, 1f, 0f);
                }
                else
                {
                    // ���ο� �̹��� ǥ�� (Resources/Image ��ο� �ִ� ������ �ҷ��� ǥ��)
                    Sprite middleSprite = Resources.Load<Sprite>("Image/" + imageName);
                    if (middleSprite != null)
                    {
                        middleImage.sprite = middleSprite;
                        middleImage.color = new Color(1f, 1f, 1f, 1f);
                    }
                }
            }


        }

    void ChangeImage()
    {
        // �̹��� ����
        BackGround_img.sprite = endImage;
        BackGround_img_shake.sprite = endImage;

        // Dotween�� ����Ͽ� �̹����� ������ ��Ÿ���� ��
        BackGround_img.DOColor(Color.white, 3f);
        BackGround_img_shake.DOColor(Color.white, 3f).OnComplete(()=> Ending());
    }
    void Ending()
    {
        ChaArea_Canvas.alpha = 1;
        TextBox_img.gameObject.SetActive(true);
        save_btn.gameObject.SetActive(false);
        load_btn.gameObject.SetActive(false);
        Invoke("LoadTitleScene",3);


    }

    void LoadTitleScene()
    {
        SceneManager.LoadScene("Title");
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



