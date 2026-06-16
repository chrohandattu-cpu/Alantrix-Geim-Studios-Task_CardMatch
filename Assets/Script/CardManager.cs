using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }

    [SerializeField]
    private GameObject prefab;
   
    [SerializeField]
    private GameObject cardList;
  
    [SerializeField]
    private Sprite cardBack;
  
    [SerializeField]
    private Sprite[] sprites;
    private Card[] cards;
    
    [SerializeField] 
    private int rows = 3;
    
    [SerializeField]
    private int columns = 6;

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject info;
    [SerializeField]
    TextMeshProUGUI matches;
    [SerializeField]
    TextMeshProUGUI turns;
    [SerializeField]
    private GameObject menu;
    private int spriteSelected;
    private int cardSelected;
    private int cardLeft;
    private bool gameStart;
    [SerializeField]
    private ToggleGroup levelToggle;
    [SerializeField] 
    private Sprite blankCardSprite;
    private Level currentLevel;
    public Sprite BlankCard()
    {
        return blankCardSprite;
    }

    void Start()
    {
        gameStart = false;
        panel.SetActive(false);
    }
    private SaveData BuildSaveData()
    {
        SaveData data = new SaveData();

        data.rows = rows;
        data.columns = columns;

        data.turns = Card.CardClick;
        data.matches = Card.CardMatch;

        data.spriteIds = new int[cards.Length];
        data.matched = new bool[cards.Length];

        for (int i = 0; i < cards.Length; i++)
        {
            data.spriteIds[i] = cards[i].SpriteID;
            data.matched[i] = cards[i].IsMatched;
        }

        return data;
    }
    private void Awake()
    {
        if (Instance != null & Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    private void RestoreBoard(SaveData data)
    {
        cardSelected = -1;
        spriteSelected = -1;

        Card.CardClick = data.turns;
        Card.CardMatch = data.matches;

        TurnText(data.turns);
        CardMatch(data.matches);

        cardLeft = cards.Length;

       
        SpriteCardAllocation();

        
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SpriteID = data.spriteIds[i];

            cards[i].Active();

           
            cards[i].Flip();
        }

      
        for (int i = 0; i < cards.Length; i++)
        {
            if (data.matched[i])
            {
                cards[i].SetMatched();
                cards[i].HideMatched();

                cardLeft--;
            }
        }

        gameStart = true;
    }
    public void SaveGame()
    {
        Debug.Log("SAVE GAME CALLED");

        if (!gameStart)
            return;

        SaveData data = BuildSaveData();

        SaveManager.Save(
            currentLevel.ToString(),
            data);
    }
    private void LoadGame(string levelKey)
    {
        SaveData data =
            SaveManager.Load(levelKey);

        if (data == null)
        {
            StartNewGame();
            return;
        }

        gameStart = true;

        HideMenue(false);

        panel.SetActive(true);

        info.SetActive(false);

        rows = data.rows;
        columns = data.columns;

        SetGamePanel();

        RestoreBoard(data);
    }
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGame();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackButton();
        }
    }
    private void HandleBackButton()
    {
        if (gameStart)
        {
            SaveGame();

            EndGame();
        }
        else
        {
            Application.Quit();
        }
    }
    
    public void SetGameLevel()
    {
        Toggle toggle =
            levelToggle.ActiveToggles().FirstOrDefault();

        currentLevel =
            toggle.GetComponent<LevelState>().level;

        SetGridSize(currentLevel);
    }
    public void StartCardGame()
    {
        string levelKey = currentLevel.ToString();

        Debug.Log("Checking save for " + levelKey);

        if (SaveManager.HasSave(levelKey))
        {
            Debug.Log("SAVE FOUND");
            LoadGame(levelKey);
        }
        else
        {
            Debug.Log("NO SAVE FOUND");
            StartNewGame();
        }
    }
    private void StartNewGame()
    {
        gameStart = true;

        HideMenue(false);

        panel.SetActive(true);

        info.SetActive(false);

        SetGamePanel();

        cardSelected = -1;
        spriteSelected = -1;

        cardLeft = cards.Length;

        SpriteCardAllocation();

        StartCoroutine(HideFace());
    }
  
    private void SetGamePanel()
    {
        int totalCards = rows * columns;

        cards = new Card[totalCards];

        foreach (Transform child in cardList.transform)
            Destroy(child.gameObject);

        RectTransform panelSize = panel.GetComponent<RectTransform>();

        float rowSize = panelSize.sizeDelta.x;
        float colSize = panelSize.sizeDelta.y;

        float xInc = rowSize / columns;
        float yInc = colSize / rows;

        float scaleX = 1f / columns;
        float scaleY = 1f / rows;

        float startX = -rowSize / 2 + xInc / 2;
        float startY = -colSize / 2 + yInc / 2;

        int index = 0;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject c = Instantiate(prefab, cardList.transform);

                c.transform.localScale =
                    new Vector3(-scaleX, scaleY, 1f);

                c.transform.localPosition =
                    new Vector3(
                        startX + j * xInc,
                        startY + i * yInc,
                        0);

                cards[index] = c.GetComponent<Card>();
                cards[index].ID = index;

                index++;
            }
        }
    }

 
    IEnumerator HideFace()
    {
      
        yield return new WaitForSeconds(0.6f);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(0.5f);
    }
   
    private void SpriteCardAllocation()
    {
        bool hasBlankCard = cards.Length % 2 != 0;

        int pairCount = cards.Length / 2;

        int[] selectedID = new int[pairCount];

        for (int i = 0; i < pairCount; i++)
        {
            int value = Random.Range(0, sprites.Length);

            for (int j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }

            selectedID[i] = value;
        }

        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }

        for (int i = 0; i < pairCount; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length);

                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }
        }

        if (hasBlankCard)
        {
            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i].SpriteID == -1)
                {
                    cards[i].SpriteID = -2;

                    Debug.Log("Blank card assigned to index " + i);

                    break;
                }
            }
        }
    }

    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
    public void BlankCardClicked(int cardId)
    {
        cards[cardId].SetMatched();

        cards[cardId].Inactive();

        cardLeft--;

        CheckGameWin();
    }
    public Sprite CardBack()
    {
        return cardBack;
    }
   
    public bool canClick()
    {
        if (!gameStart)
            return false;
        return true;
    }
   
    public void cardClicked(int spriteId, int cardId)
    {
       
        if (spriteSelected == -1)
        {
            spriteSelected = spriteId;
            cardSelected = cardId;
        }
        else
        { 
            Card.CardClick++;
            TurnText(Card.CardClick);
            if (spriteSelected == spriteId)
            {
                Card.CardMatch++;
                CardMatch(Card.CardMatch);

                cards[cardSelected].SetMatched();
                cards[cardId].SetMatched();

                cards[cardSelected].Inactive();
                cards[cardId].Inactive();

                cardLeft -= 2;

                CheckGameWin();
            }
            else
            {
                
                cards[cardSelected].Flip();
                cards[cardId].Flip();
            }
            cardSelected = spriteSelected = -1;
        }
    }
   
    private void CheckGameWin()
    {
       
        if (cardLeft == 0)
{
    SaveManager.DeleteSave(
        currentLevel.ToString());

    EndGame();

            AudioManager.Instance.PlaySound(1);
}
    }
  
    private void EndGame()
    {
        ResetText();
        gameStart = false;
        HideMenue((gameStart ? false : true));
        panel.SetActive(false);
    }
    public void Home()
    {
        HandleBackButton();
        ResetText();
        EndGame();
    }
    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }
    
    private void GridLayout(int x, int y) { 
        rows= x;
        columns = y;
    }
    private void SetGridSize(Level level)
    {
        switch ( level)
        {
            case Level.Level1:
                GridLayout(2, 2);
                break;

            case Level.Level2:
                GridLayout(2, 3);
                break;

            case Level.Level3:
                GridLayout(3, 3);
                break;

            case Level.Level4:
                GridLayout(4, 3);
                break;

            case Level.Level5:
                GridLayout(4, 4);
                break;

            case Level.Level6:
                GridLayout(5, 3);
                break;
           
            case Level.Level7:
                GridLayout(5, 4);
                break;
            
            case Level.Level8:
                GridLayout(5, 5);
                break;
        }
    }

    
    private void HideMenue(bool hide)
    {
        menu.SetActive(hide);
    }
   
    private void TurnText(int turnCount) { turns.text = turnCount.ToString(); }
    private void CardMatch(int match) { matches.text = match.ToString(); }
   
    private void ResetText()
    {
        Card.CardClick = Card.CardMatch= 0;
        TurnText(0);
        CardMatch(0);
    }
}
