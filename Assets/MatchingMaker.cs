using Firebase.Auth;
using Firebase.Database;
using Lean.Gui;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MatchingMaker : MonoBehaviour
{
    //private FirebaseManager fbManager;
    private PersistentDataManager dataManager;
    private AudioManager audioManager;
    private FirebaseUser user;
    private string OwnerId;
    private string OpponentId;
    public UnityEvent FoundMatch;

    public float timeCount = 0.0f;
    public Text TimerText;
    public RectTransform TimerBox;
    public LeanButton button;
    public Text ButtonText;
    public Image buttonColor;
    public Color DisableButtonColor;
    public Color EnableButtonColor;

    private bool IsSearching = false;


    void Update()
    {
        //if (IsSearching)
        //{
        //    timeCount += Time.deltaTime;            
        //    TimerText.text = (timeCount).ToString("00");            
        //}
    }

    private void Awake()
    {
        //fbManager = FindObjectOfType<FirebaseManager>();
        dataManager = FindObjectOfType<PersistentDataManager>();
        audioManager = FindObjectOfType<AudioManager>();
        //user = fbManager.Auth.CurrentUser;
        audioManager.Stop("BgMusic");
    }

    /*
    public async void StartMachingMaker()
    {
        if (IsSearching)
        {
            ButtonText.text = "Play";
            buttonColor.color = EnableButtonColor;
            IsSearching = false;
            timeCount = 0.0f;
            LeanTween.moveLocalX(button.gameObject, 502, 0.5f);
            LeanTween.moveLocalX(TimerBox.gameObject, 501, 0.5f);
            await fbManager.DB.GetReference(FirebasePaths.CHALLENGES).Child(user.UserId).RemoveValueAsync();
        }
        else
        {
            ButtonText.text = "Cancel";
            buttonColor.color = DisableButtonColor;
            LeanTween.moveLocalX(button.gameObject, 372, 0.5f);
            LeanTween.moveLocalX(TimerBox.gameObject, 645, 0.5f);

            IsSearching = true;


            DataSnapshot myMach = await fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(user.UserId).GetValueAsync();
            if (myMach.Exists)
            {
                Debug.Log("myMach: " + myMach.GetRawJsonValue());
                OpponentId = ((IDictionary)myMach.Value)["OpponentID"].ToString();
                GoToMatch(user.UserId, true);
                return;
            }

            DataSnapshot opponentMach = await fbManager.DB.GetReference(FirebasePaths.MATCHES).OrderByChild("OpponentID").EqualTo(user.UserId).LimitToLast(1).GetValueAsync();
            if (opponentMach.Exists)
            {
                Debug.Log("opponentMach: " + opponentMach.GetRawJsonValue());
                OpponentId = user.UserId;
                var opKey = "";
                foreach (var opChild in opponentMach.Children)
                {
                    opKey = opChild.Key;
                }
                GoToMatch(opKey, true);
                return;
            }

            DataSnapshot data = await fbManager.DB.GetReference(FirebasePaths.CHALLENGES).OrderByValue().LimitToLast(1).GetValueAsync();

            if (data.Value == null)
            {
                await CreateChallenge();
                return;
            }


            var key = "";
            foreach (var child in data.Children)
            {
                key = child.Key;
            }

            Debug.Log(key);
            if (key == user.UserId)
            {
                await CreateChallenge();
            }
            else
            {
                await AcceptChallenge(key);
            }
        }
    }

    private void GoToMatch(string key, bool exists = false)
    {
        OwnerId = key;
        fbManager.PeristentData["Exists"] = exists;
        fbManager.PeristentData["OwnerId"] = OwnerId;
        fbManager.PeristentData["OpponentId"] = OpponentId;
        Debug.Log($"MMK - Ow: {OwnerId}, Op: {OpponentId}");
        FoundMatch.Invoke();
    }

    private async Task CreateChallenge()
    {
        Dictionary<string, object> challenge = new Dictionary<string, object>
        {
            { user.UserId, ServerValue.Timestamp }
        };
        await fbManager.DB.GetReference(FirebasePaths.CHALLENGES).SetValueAsync(challenge);
        fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(user.UserId).Child("OpponentID").ValueChanged += (object sender, ValueChangedEventArgs e) =>
        {
            if (e.Snapshot.Exists)
            {
                OpponentId = e.Snapshot.Value.ToString();

                Debug.Log("OpponentId:" + OpponentId);
                GoToMatch(user.UserId);
            }
        }; 
    }

    private async Task AcceptChallenge(string key)
    {
        await fbManager.DB.GetReference(FirebasePaths.CHALLENGES).Child(key).RemoveValueAsync();
        bool IsCompleted = false;
        Dictionary<string, object> match = new Dictionary<string, object>
        {
            { "OpponentID", user.UserId }
        };
        OpponentId = user.UserId;
        await fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(key).SetValueAsync(match).ContinueWith(t => {
            IsCompleted = t.IsCompleted;
        });
        if (IsCompleted)
        {
            GoToMatch(key);
        }
    }

    public void DeleteMyMatch()
    {
        fbManager.DB.GetReference(FirebasePaths.MATCHES).Child(user.UserId).RemoveValueAsync();
    }
    */

    public void StartPractice()
    {
        OpponentId = "IA";
        //fbManager.PeristentData["Exists"] = false;
        //fbManager.PeristentData["OwnerId"] = fbManager.Auth.CurrentUser.UserId; //ONLINE
        //fbManager.PeristentData["OpponentId"] = OpponentId;
        dataManager["Exists"] = false;
        dataManager["UserId"] = "PLAYER";
        dataManager["OwnerId"] = "PLAYER";
        dataManager["OpponentId"] = "IA";

        Debug.Log($"MMK - Ow: {OwnerId}, Op: {OpponentId}");
        FoundMatch.Invoke();
    }

    void OnDisable()
    {
        
    }

}
