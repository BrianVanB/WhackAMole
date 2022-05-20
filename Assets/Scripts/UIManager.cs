using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//The UI manager deals with showing the correct canvas at different points in the game
public class UIManager : MonoBehaviour
{
    public Canvas startCanvas;  //Canvas for the start screen
    public Canvas gameCanvas;   //Canvas for in-game UI
    public Canvas endCanvas;    //Canvas for game-over screen

    //Access to the sliders in the start screen in order to use their values
    public Slider holeSlider;
    public Slider timeSlider;

    //Access to the text which displays the final score
    public Text finalScoreText;

    private GameController gameController;

    void Start()
    {
        gameController = FindObjectOfType<GameController>();
        ReturnToStart();
    }

    //Sets the UI tot the starting state, showing the start screen
    public void ReturnToStart()
    {
        startCanvas.enabled = true;
        gameCanvas.enabled = false;
        endCanvas.enabled = false;
    }

    //Startgame switches the UI from start screen to in game UI and sends the slider values
    //to the gameController
    public void StartGame()
    {
        startCanvas.enabled = false;
        gameCanvas.enabled = true;

        gameController.StartGame((int)holeSlider.value, (int)timeSlider.value);
    }

    //EndGame is called once the player's time has run out and will switch from the
    //in-game UI to the end screen, and shows the player what score they got and if it's a PB
    public void EndGame(bool newHs, int score)
    {
        gameCanvas.enabled=false;
        endCanvas.enabled=true;

        finalScoreText.text = $"{(newHs ? "New Best!\n" : "")}Final Score: {score}";
    }

    //Updates the text of the slider to show the user what value they have selected
    public void UpdateHoleText()
    {
        holeSlider.GetComponentInChildren<Text>().text = $"Holes: {holeSlider.value}";
    }

    //Updates the text of the slider to show the user what value they have selected
    public void UpdateTimeText()
    {
        timeSlider.GetComponentInChildren<Text>().text = $"Time: {timeSlider.value} seconds";
    }
}
