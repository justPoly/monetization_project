using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Google;
using UnityEngine.UI;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class GoogleAuthentication : MonoBehaviour
{
    public string imageURL, username, email;

    public TMP_Text userNameText, userEmailText;

    public Image profilePic;

    public GameObject loginPanel;

    public string webClientId = "883892493947-v6s268kg735mqiq2h4s3ik9b3ro51qun.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;
    private FirebaseAuth auth;
    private DatabaseReference databaseReference;

    void Awake()
    {
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true
        };

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        });
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(OnAuthenticationFinished, TaskScheduler.FromCurrentSynchronizationContext());
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task)
    {
        if (task.IsFaulted)
        {
            using (IEnumerator<System.Exception> enumerator = task.Exception.InnerExceptions.GetEnumerator())
            {
                if (enumerator.MoveNext())
                {
                    GoogleSignIn.SignInException error = (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);
                }
                else
                {
                    Debug.LogError("Got Unexpected Exception?!?" + task.Exception);
                }
            }
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Canceled");
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");

            userNameText.text = task.Result.DisplayName;
            userEmailText.text = task.Result.Email;

            imageURL = task.Result.ImageUrl.ToString();
            loginPanel.SetActive(false);

            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
            {
                if (authTask.IsCompleted)
                {
                    FirebaseUser newUser = authTask.Result;
                    Debug.Log("User signed in successfully: " + newUser.DisplayName + " (" + newUser.UserId + ")");
                    // You can now save progress or load existing progress
                    LoadProgress(newUser.UserId);
                }
                else
                {
                    Debug.LogError("Firebase authentication failed: " + authTask.Exception);
                }
            });

            StartCoroutine(LoadProfilePic());
        }
    }

    IEnumerator LoadProfilePic()
    {
        WWW www = new WWW(imageURL);
        yield return www;

        profilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
    }

    public void OnSignOut()
    {
        userNameText.text = "";
        userEmailText.text = "";

        imageURL = "";
        loginPanel.SetActive(true);

        Debug.Log("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();

        auth.SignOut();
    }

    public void SaveProgress(string userId, string progressData)
    {
        databaseReference.Child("users").Child(userId).Child("progress").SetValueAsync(progressData);
    }

    public void LoadProgress(string userId)
    {
        databaseReference.Child("users").Child(userId).Child("progress").GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                string progressData = snapshot.Value?.ToString();
                Debug.Log("Loaded progress: " + progressData);
                // Handle loaded progress data
            }
            else
            {
                Debug.LogError("Failed to load progress: " + task.Exception);
            }
        });
    }

    void Update()
    {
        // Optionally handle Firebase updates if needed
    }

}