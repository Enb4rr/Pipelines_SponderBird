using System.Collections;
using UnityEngine;
using NUnit.Framework;
using UnityEngine.TestTools;

[TestFixture]
public class GameFlowTests
{
    private GameObject gameManagerObj;
    private GameManager gameManager;
    
    private GameObject birdObj;
    private BirdController bird;

    private Rigidbody2D birdRb;
    
    private GameObject spawnerObj;
    private PipeSpawner pipeSpawner;

    private GameObject scoreManagerObj;
    private ScoreManager scoreManager;

    [UnitySetUp]
    public IEnumerator SetUp()
    {
        PlayerPrefs.DeleteKey("FlappyHighScore");
        
        scoreManagerObj = new GameObject("ScoreManager");
        scoreManager = scoreManagerObj.AddComponent<ScoreManager>();
        
        birdObj = new GameObject("Bird");
        birdObj.AddComponent<CircleCollider2D>();
        birdRb = birdObj.AddComponent<Rigidbody2D>();
        birdRb.gravityScale = 0;
        bird = birdObj.AddComponent<BirdController>();
        
        spawnerObj = new GameObject("PipeSpawner");
        pipeSpawner = spawnerObj.AddComponent<PipeSpawner>();
        
        gameManagerObj = new GameObject("GameManager");
        gameManager = gameManagerObj.AddComponent<GameManager>();
        
        SetPrivateField(gameManager, "bird", bird);
        SetPrivateField(gameManager, "pipeSpawner", pipeSpawner);

        yield return null;
    }

    [TearDown]
    public void TearDown()
    {
        if (gameManagerObj != null) Object.Destroy(gameManagerObj);
        if (birdObj != null) Object.Destroy(birdObj);
        if (spawnerObj != null) Object.Destroy(spawnerObj);
        if (scoreManagerObj != null) Object.Destroy(scoreManagerObj);

        foreach (var pipe in Object.FindObjectsByType<Pipe>(FindObjectsSortMode.None))
        {
            Object.Destroy(pipe.gameObject);
        }
    }

    [UnityTest]
    public IEnumerator Game_StartsInIdleState()
    {
        yield return null;
        
        Assert.AreEqual(GameManager.GameState.Idle, gameManager.State, "Game should begin in Idle State");
    }

    [UnityTest]
    public IEnumerator Score_StartsAtZero()
    {
        yield return null;
        
        Assert.AreEqual(0, scoreManager.GetCurrentScore(), "Score should be zero");
    }

    [UnityTest]
    public IEnumerator OnBirdDied_TransitionsToGameOver()
    {
        SetState(gameManager, GameManager.GameState.Playing);
        yield return null;
        
        gameManager.OnBirdDied();
        yield return null;
        
        Assert.AreEqual(GameManager.GameState.GameOver, gameManager.State, "OnBirdDied should set state to GameOver");
    }

    [UnityTest]
    public IEnumerator OnBirdDied_CalledTwice_DoesNotCrash()
    {
        SetState(gameManager, GameManager.GameState.Playing);
        yield return null;
        
        gameManager.OnBirdDied();
        gameManager.OnBirdDied();
        
        yield return null;
        
        Assert.AreEqual(GameManager.GameState.GameOver, gameManager.State, "Double OnBirdDied should not corrupt state");
    }

    [UnityTest]
    public IEnumerator AddPoint_DuringPlay_IncreasesScore()
    {
        yield return null;
        scoreManager.ResetScore();
        scoreManager.AddPoint();
        scoreManager.AddPoint();
        
        Assert.AreEqual(2, scoreManager.GetCurrentScore(), "Score should be 1");
    }

    [UnityTest]
    public IEnumerator HighScore_PersistsBetweenRounds()
    {
        yield return null;
        scoreManager.ResetScore();

        for (int i = 0; i < 5; i++)
        {
            scoreManager.AddPoint();
        }
        
        scoreManager.SaveHighScore();
        
        Assert.AreEqual(5, scoreManager.GetHighScore(), "High score should be 5");
        
        scoreManager.ResetScore();

        for (int i = 0; i < 3; i++)
        {
            scoreManager.AddPoint();
        }
        
        scoreManager.SaveHighScore();
        
        Assert.AreEqual(5, scoreManager.GetHighScore(), "High score should still be 5 after lower score attempt");
    }
    
    private void SetPrivateField(object target, string fieldName, object value)
    {
        var field = target.GetType().GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        field?.SetValue(target, value);
    }

    private void SetState(GameManager gm, GameManager.GameState state)
    {
        var field = typeof(GameManager).GetField("<State>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        field?.SetValue(gm, state);
    }
}
