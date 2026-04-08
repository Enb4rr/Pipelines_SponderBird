using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    // Singleton
    public static FirebaseManager Instance { get; private set; }
    
    // Variables

    public bool IsAuthenticated { get; private set; } = false;
    public string UserId { get; private set; } = "";
    public string DisplayName { get; private set; } = "Player";
    public string IdToken { get; private set; } = "";
    public string ProjectId { get; private set; } = "";
    
    #if UNITY_WEBGL && !UNITY_EDITOR
    
    [DllImport("__Internal")] private static extern void InitFirebaseBridge();
    [DllImport("__Internal")] private static extern void SubmitScoreToFirestore(string jsonBody);
    
    #else
    
    private static void InitFirebaseBridge() => Debug.Log("InitFirebaseBridge Stub");
    private static void SubmitScoreToFirestore(string jsonBody) => Debug.Log("SubmitScoreToFirestore Stub");
    
    #endif

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    public void OnAuthReceived(string json)
    {
        Debug.Log($"Auth Received: {json}");

        var data = JsonUtility.FromJson<AuthPayload>(json);
        UserId = data.uid;
        IdToken = data.idToken;
        DisplayName = data.displayName;
        ProjectId = data.projectId;
        IsAuthenticated = !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(IdToken);
        
        Debug.Log($"User Authenticated as {DisplayName}, UID: {UserId}");
    }

    public void SubmitScore(int score, int pipes, int duration)
    {
        if (!IsAuthenticated)
        {
            Debug.Log("Not Authenticated, Score not submitted");
            return;
        }

        var payload = new ScorePayload
        {
            score = score,
            pipes = pipes,
            duration = duration
        };
        
        string json = JsonUtility.ToJson(payload);
        SubmitScoreToFirestore(json);
    }
}