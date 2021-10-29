using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;
    public GameObject[] slots;
    public GameObject[] sprites;
    private int activeSlot = 0; 
    private Color highlight = Color.green;
    private Color defaultColor = Color.white;
    
    [Header("Scores")]
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI p1ScoreText;
    public TextMeshProUGUI p2ScoreText;
    public TextMeshProUGUI p3ScoreText;
    public TextMeshProUGUI p4ScoreText;

    void Awake()
    {
        instance = this;
    }

    // The player Starts rolling or keeps rolling 
    public void OnRollButton()
    {
        UpdateSpriteBar(GameManager.instance.dieValue);
        GameManager.instance.AddCurrentScore();
        GameManager.instance.DiceReset();
        GameManager.instance.Roll();
        UpdateScoreText(GameManager.instance.p1Score, GameManager.instance.p2Score, GameManager.instance.p3Score, GameManager.instance.p4Score, GameManager.instance.player, GameManager.instance.currentScore);
    }

    // The player keeps the current score they have and "passes the dice" to the next player
    public void OnStopButton()
    {
        GameManager.instance.Stop();
        ClearSpriteBar();
    }

    public void UpdateScoreText(int p1Score, int p2Score, int p3Score, int p4Score, int player, int currentScore)
    {
        currentScoreText.text = "Current Score: " + currentScore;
        p1ScoreText.text = "P1 Score: " + p1Score;
        p2ScoreText.text = "P2 Score: " + p2Score;
        p3ScoreText.text = "P3 Score: " + p3Score;
        p4ScoreText.text = "P4 Score: " + p4Score;

        if(player == 1)
        {
            p1ScoreText.color = highlight;
            p2ScoreText.color = defaultColor;
            p3ScoreText.color = defaultColor;
            p4ScoreText.color = defaultColor;
        }
        else if(player == 2)
        {
            p2ScoreText.color = highlight;
            p1ScoreText.color = defaultColor;
            p3ScoreText.color = defaultColor;
            p4ScoreText.color = defaultColor;
        }
        else if(player == 3)
        {
            p3ScoreText.color = highlight;
            p1ScoreText.color = defaultColor;
            p2ScoreText.color = defaultColor;
            p4ScoreText.color = defaultColor;
        }
        else if(player == 4)
        {
            p4ScoreText.color = highlight;
            p1ScoreText.color = defaultColor;
            p2ScoreText.color = defaultColor;
            p3ScoreText.color = defaultColor;
        }
    }

    public void UpdateSpriteBar(int[] spriteIndex) 
    {
        if(activeSlot >= slots.Length)
        {
            foreach(GameObject slot in slots)
            {
                if(slot.transform.childCount > 0)
                {
                    Destroy(slot.gameObject.transform.GetChild(0).gameObject);
                }
            }

            activeSlot = 0; 
        }

        for(int i = 0; i < spriteIndex.Length; i++)
        {
            spriteIndex[i]--;

            if(GameManager.instance.dice[i].gameObject.layer == 8 && GameManager.instance.dice[i].gameObject.activeSelf && activeSlot <= 5) 
            {
                Instantiate(sprites[spriteIndex[i]], slots[activeSlot].transform);
                activeSlot++;
            }
        }
    }

    public void ClearSpriteBar()
    {
        foreach(GameObject slot in slots)
        {
            if(slot.transform.childCount > 0)
            {
                Destroy(slot.gameObject.transform.GetChild(0).gameObject);
            }
        }

        activeSlot = 0;
    }
}