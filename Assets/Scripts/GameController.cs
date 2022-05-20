using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    //Variables set by the player in start screen
    private float timeRemaining;    //Game time in seconds
    private int numberOfHoles;      //Number of holes

    //Access to the hole model and a parent container which will have all the holes
    public GameObject HolePrefab;
    public Transform HoleContainer;

    //Offset between the holes in the grid
    [Tooltip("Should be at least the width of the hole model")]
    public int offsetX;
    [Tooltip("Should be at least the length of the hole model")]
    public int offsetY;

    //Access to the text elements of the game UI so they can be updated
    public Text timeText;
    public Text scoreText;

    //Game stats
    private bool running = false;   //Indicates whether the game is active
    private int currentScore;       //The current score of the player
    private int highScore;          //The high score

    void Start()
    {
        highScore = LoadHighScore();
    }

    void Update()
    {
        if (Running)
        {
            timeRemaining -= Time.deltaTime;
            timeText.text = $"Time left: {Mathf.FloorToInt(timeRemaining)}";

            if (timeRemaining < 0)
                EndGame();
        }
    }

    //Builds the holes and sets all game parameters to their start value
    public void StartGame(int holes, int seconds)
    {
        numberOfHoles = holes;
        timeRemaining = seconds;

        BuildGrid();
        running = true;
        currentScore = 0;
        scoreText.text = $"Score: {currentScore}";
    }

    //Performs removal of the holes, updates the highscore on a new best and tells the UI to show endscreen
    public void EndGame()
    {
        running = false;

        if (currentScore > highScore)
            SaveHighScore(currentScore);

        FindObjectOfType<UIManager>().EndGame(currentScore>highScore, currentScore);

        for(int i = 0; i < HoleContainer.childCount;i++)
            Destroy(HoleContainer.GetChild(i).gameObject);
    }

    //Builds the grid of holes at the start of a game
    void BuildGrid()
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(numberOfHoles));

        //Try to keep grid somewhat centered
        float width = (gridSize * 2.4f) + (gridSize - 1) * offsetX;
        float height = (gridSize * 2.4f) + (gridSize - 1) * offsetY;
        float startX = -(width / 4f);
        float startY = -(height / 4f);

        int spawnedHoles = 0;
        for (int y = 0; y < gridSize; y++)
            for (int x = 0; x < gridSize; x++)
            {
                if (spawnedHoles == numberOfHoles)
                    continue;

                Vector3 position = new Vector3(startX + x * offsetX, 0, startY + y * offsetY);

                GameObject hole = Instantiate(HolePrefab, position, Quaternion.identity);
                hole.transform.Rotate(new Vector3(-90, 180, 0));
                hole.transform.SetParent(HoleContainer);
                spawnedHoles++;
            }
    }

    //Increases score, keeping combo in mind, and updating the score display
    public void IncreaseScore(int combo)
    {
        //Give the player bonus points for every 3 consecutive hits
        if (combo % 3 == 0)
            currentScore += 150;
        else
            currentScore += 15;

        scoreText.text = $"Score: {currentScore}";
    }

    //Saves the highscore to the player prefs
    private void SaveHighScore(int score)
    {
        PlayerPrefs.SetInt("Highscore", score);
    }

    //Reads the highscore from the playerprefs
    private int LoadHighScore()
    {
        return PlayerPrefs.GetInt("Highscore", 0);
    }

    public bool Running { get { return running; } }
}
