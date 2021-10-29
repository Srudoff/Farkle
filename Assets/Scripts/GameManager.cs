using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; 
    public Dice[] diceScript; 
    public GameObject[] dice; 
    public int[] dieValue; 
    public Vector3[] diceStartPos; 
    public int potentialScore = 0;
    public int currentScore = 0; 
    public bool farkle;
    private int diceOnHold = 0; 
    private int diceRolled = 0;
    private int combo = 0; 
    private int comboScore = 0;
    private int baseScore = 0;
    private int amountOfCombos = 0; 
    private int amountOfOnes = 0;
    private int amountOfTwos = 0;
    private bool sixDice = false;
    private bool isTwo = false;
    private bool isThree = false;
    private bool isFour = false; 
    private bool checkForFarkle = false;

    [Header("Player Scores")] 
    public int player;
    public int p1Score;
    public int p2Score;
    public int p3Score;
    public int p4Score;
    


    private void Awake()
    {
        instance = this;
        dice = GameObject.FindGameObjectsWithTag("Dice");
        Dice[] diceScript = new Dice[6];
    }

    void Start()
    {
        // Starting player is picked at random
        player = Random.Range(1, 5);
        
        for(int i = 0; i < dice.Length; i++)
        {
            diceStartPos[i] = dice[i].transform.position;
            diceScript[i] = dice[i].GetComponent<Dice>();
            dice[i].gameObject.layer = 8;
            dice[i].gameObject.SetActive(false);
        }

        diceOnHold = dice.Length;

        GameUI.instance.UpdateScoreText(p1Score, p2Score, p3Score, p4Score, player, currentScore);
    }

    void Update()
    {
        for(int i = 0; i < dice.Length; i++)
        {
            if(!dice[i].gameObject.activeSelf)
            {
                dieValue[i] = 0;
            }
            else
            {
                dieValue[i] = diceScript[i].dieValue;               
            }
        }

        foreach(Dice script in diceScript)
        {
            if(script.onBottom && script.dieValue > 0 && !checkForFarkle)
            {
                diceRolled++;
            }
        }

        if(diceRolled == 6 && checkForFarkle == false)
        {
            diceRolled = 0;
            Invoke("CheckForFarkle", 4.0f);
            checkForFarkle = true;
        }
    }

    // Add the player's current score to their total score, or 0 if Farkle. Next player's turn begins.
    public void Stop()

    {
        if(!farkle)
        {
            AddCurrentScore(); 
        }
        else
        {
            farkle = false;
        }

        // Make all dice inactive until next player's turn
        foreach(GameObject die in dice)
        {
            die.gameObject.layer = 8;
            die.gameObject.SetActive(false);
            diceOnHold = dice.Length;
        }

        foreach(Dice script in diceScript)
        {
            script.myRenderer.material.color = script.startColor;
        }

        AddTotalScore(player);
        ChangePlayer();
        GameUI.instance.UpdateScoreText(p1Score, p2Score, p3Score, p4Score, player, currentScore);
        currentScore = 0;
    }

    public void Roll()
    {
        // Adds an imulse to the dice as you roll - just for aesthetics
        foreach(GameObject die in dice)
        {
            Rigidbody rb = die.GetComponent<Rigidbody>();
            rb.AddForce(2, 0, 0, ForceMode.Impulse);
        }

        Invoke("CorrectionRoll", 3.0f); 
    }

    public void CorrectionRoll() // CorrectionRoll is meant to "bump" a die if it lands leaning against something
    {
        for(int j = 0; j < diceScript.Length; j++)
        {
            Rigidbody rb = dice[j].GetComponent<Rigidbody>();

            if(diceScript[j].dieValue <= 0 && diceScript[j].onBottom)
            {
                rb.AddForce(0, 4, 0, ForceMode.Impulse);   
            }
        }
    }

    public void DiceReset()
    {
        checkForFarkle = false;
        amountOfCombos = 0;
        amountOfOnes = 0;
        amountOfTwos = 0;
        sixDice = false;
        baseScore = 0;
        comboScore = 0; 
        potentialScore = 0;

        foreach(Dice script in diceScript)
        {
            script.comboCheck = false;
            script.farkleCheck = false;
        }

        if(diceOnHold == dice.Length) // If all dice are on hold but the player hasn't farkled, the player can roll them all again
        {
            for(int j = 0; j < dice.Length; j++)
            {
                dice[j].gameObject.SetActive(true);
                dice[j].gameObject.layer = 0; 
                diceScript[j].myRenderer.material.color = diceScript[j].startColor; 
                diceOnHold = 0;
            }
        }

        for(int i = 0; i < dice.Length; i++)
        {
            if(dice[i].gameObject.layer == 0)
            {
                dice[i].transform.position = diceStartPos[i];
                dice[i].transform.rotation = Random.rotation; 
            }
            else if(dice[i].gameObject.layer == 8 && dice[i].gameObject.activeSelf)
            {
                dice[i].gameObject.SetActive(false);
                diceOnHold++; 
            }
        }
    }

    public void AddCurrentScore() 
    {
        for(int i = 0; i < dice.Length; i++) // Cycle through each die
        {
            if(dice[i].gameObject.layer == 8 && diceScript[i].comboCheck == false && dice[i].gameObject.activeSelf) // Check if they're on hold
            { 
                for(int j = 0; j < dice.Length; j++) // If not, cycle through to compare values
                {
                    if(dieValue[i] == dieValue[j] && dice[j].gameObject.activeSelf && dice[j].gameObject.layer == 8) // Checks if die is selected
                    {
                        combo++;  
                        diceScript[i].comboCheck = true; // Prevents dice from being checked twice
                        diceScript[j].comboCheck = true;
                    }
                }
                
                CalculateCombos(combo, dieValue[i]); // Checks for 6-die combos such as 3 pairs, or straights
                BaseScore(combo, dieValue[i]); // Uses base scoring such as 1s (100), 5s (50), 3 of a kind (100*dieValue)

                combo = 0; // Resets the combo for the next iteration of the [j] loop
            }
        }

        if(!sixDice)
        {
            if(baseScore > 0)
            {
                currentScore += baseScore;
            }
            else
            {
                currentScore = 0;
            }
        }
        else
        {
            currentScore += comboScore;
        }
    }

    public void CalculateCombos(int combo, int value)
    {
        // combo represents the amount of dice with the same value (in a roll with 2 3s, combo would be 2) 
        // amountOfCombos represents how many different combos there are - (in a 3 pairs roll, amountOfCombos would be 3)
        if(combo == 1)
        {
            amountOfCombos++; 
            amountOfOnes++;

            if(amountOfOnes == 6) 
            {
                comboScore += 1500;
                sixDice = true;
            }
        }
        else if(combo == 2)
        {
            amountOfCombos++;
            amountOfTwos++;
            // isTwo and isFour are for 3 pair combos that consist of a 4 combo and a 2 combo ()
            // ex. 4 2s and 2 6s is 3 pairs (1500 points). If it were scored in BaseScore, it would only give 1000 points
            isTwo = true; 

            if(amountOfTwos == 3)
            {
                comboScore += 1500;
                sixDice = true;
            }
            else if(isFour)
            {
                comboScore += 1500;
                sixDice = true;
                isFour = false;
            }
        }
        else if(combo == 3)
        {

            amountOfCombos++;

            if(amountOfCombos == 2 && isThree)
            {
                comboScore += 2500;
                sixDice = true;
                isThree = false;
            }

            isThree = true; // Having this at the end prevents other combos from incrementing amountOfCombos and scoring 2500 if the 3 combo was last to run
        }
        else if(combo == 4)
        {
            // isTwo and isFour are for 3 pair combos that consist of a 4 combo and a 2 combo ()
            // ex. 4 2s and 2 6s is 3 pairs (1500 points). If it were scored in BaseScore, it would only give 1000 points
            isFour = true;

            if(isTwo)
            {
                comboScore += 1500;
                sixDice = true;
                isTwo = false;
            }
        }
        else if(combo == 6)
        {
            comboScore += 3000;
            sixDice = true;
        }
    }

    public void BaseScore(int combo, int value)
    {
        if(combo == 3)
        {
            if(value > 1)
            {
                baseScore += value * 100;
            }
            else
            {
                baseScore += 300;
            }
        }
        else if(combo == 4)
        {
            baseScore += 1000;
        }
        else if(combo == 5) 
        {
            baseScore += 2000;
        }
        else if(combo == 1 || combo == 2)
        {
            if(value == 1)
            {
                baseScore += 100 * combo;
            }
            else if(value == 5)
            {
                baseScore += 50 * combo;
            }
        }
        else
        {
            farkle = true;
        }
    }

    public void AddTotalScore(int player) // Track each player's individual game scores
    {
        if(player == 1)
        {
            if(p1Score == 0 && currentScore >= 500) // Players must get at least 500 points on their first turn to enter the game
            {
                p1Score += currentScore;
            }
            else
            {
                currentScore = 0;
            }
        }
        else if(player == 2)
        {
            if(p2Score == 0 && currentScore >= 500)
            {
                p2Score += currentScore;
            }
            else
            {
                currentScore = 0;
            }
        }
        else if(player == 3)
        {
            if(p3Score == 0 && currentScore >= 500)
            {
                p3Score += currentScore;
            }
            else
            {
                currentScore = 0;
            }
        }
        else if(player == 4)
        {
            if(p4Score == 0 && currentScore >= 500)
            {
                p4Score += currentScore;
            }
            else
            {
                currentScore = 0;
            }        
        }
    }

// CheckForFarkle runs similar logic to AddCurrentScore, only does not save score - used only to automatically check each roll for a farkle
    public void CheckForFarkle() // AddCurrentScore must be run by clicking the roll button - this checks automatically for Farkles
    {
        for(int i = 0; i < dice.Length; i++)
        {
            if(diceScript[i].dieValue > 0 && dice[i].gameObject.activeSelf && diceScript[i].farkleCheck == false)
            {
                for(int j = 0; j < dice.Length; j++) 
                {
                    if(dieValue[i] == dieValue[j] && dice[j].gameObject.activeSelf)
                    {
                        combo++;
                        diceScript[i].farkleCheck = true; 
                        diceScript[j].farkleCheck = true;
                    }
                }
                
                CalculateCombos(combo, dieValue[i]);
                BaseScore(combo, dieValue[i]);

                combo = 0;
            } 
        }

        if(!sixDice)
        {
            if(baseScore > 0)
            {
                potentialScore += baseScore;
            }
            else
            {
                potentialScore = 0;
                Farkle();
            }
        }
        else
        {
            potentialScore += comboScore;
        }

        baseScore = 0;
        comboScore = 0;
    }

    public void Farkle()
    {
        GameUI.instance.Farkle();
    }

    public void ChangePlayer()
    {
        if(player < 4)
        {
            player++;
        }
        else
        {
            player = 1;
        }
    }
}