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
            if (task.Result == DependencyStatus.Available)
            {
                FirebaseApp app = FirebaseApp.DefaultInstance;
                auth = FirebaseAuth.DefaultInstance;
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;

                // Check if there is already a signed-in user
                if (auth.CurrentUser != null)
                {
                    // User is signed in, handle accordingly
                    FirebaseUser currentUser = auth.CurrentUser;
                    HandleSignedInUser(currentUser);
                }
                else
                {
                    // No user is signed in, show login panel
                    loginPanel.SetActive(true);
                }
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
                // Optionally handle this case
            }
        });
    }

    void Update()
    {
        // Optionally handle Firebase updates if needed
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
            foreach (var exception in task.Exception.InnerExceptions)
            {
                if (exception is GoogleSignIn.SignInException signInException)
                {
                    Debug.LogError("Got Error: " + signInException.Status + " " + signInException.Message);
                }
                else
                {
                    Debug.LogError("Got Unexpected Exception: " + exception);
                }
            }
            loginPanel.SetActive(true);  // Ensure login panel is active
        }
        else if (task.IsCanceled)
        {
            Debug.LogError("Canceled");
            loginPanel.SetActive(true);  // Ensure login panel is active
        }
        else
        {
            Debug.Log("Welcome: " + task.Result.DisplayName + "!");

            userNameText.text = task.Result.DisplayName;
            userEmailText.text = task.Result.Email;

            imageURL = task.Result.ImageUrl.ToString();

            Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(task.Result.IdToken, null);
            auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
            {
                if (authTask.IsCompleted && !authTask.IsFaulted && !authTask.IsCanceled)
                {
                    FirebaseUser newUser = authTask.Result;
                    Debug.Log("User signed in successfully: " + newUser.DisplayName + " (" + newUser.UserId + ")");
                    
                    // Set login panel to inactive here
                    loginPanel.SetActive(false);

                    // You can now save progress or load existing progress
                    HandleSignedInUser(newUser);
                }
                else
                {
                    Debug.LogError("Firebase authentication failed: " + authTask.Exception);
                    loginPanel.SetActive(true);  // Ensure login panel is active in case of failure
                }
            });
        }
    }

    IEnumerator LoadProfilePic()
    {
        WWW www = new WWW(imageURL);
        yield return www;

        if (www.error == null)
        {
            profilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        }
        else
        {
            Debug.LogError("Failed to load profile picture: " + www.error);
        }
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

    public void OnGuestSignIn()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(authTask =>
        {
            if (authTask.IsCompleted && !authTask.IsFaulted && !authTask.IsCanceled)
            {
                Firebase.Auth.AuthResult result = authTask.Result;
                FirebaseUser newUser = result.User;
                Debug.Log("Guest user signed in successfully: " + newUser.UserId);

                // Set login panel to inactive here
                loginPanel.SetActive(false);

                userNameText.text = "Guest";
                userEmailText.text = "";

                // Load existing progress or handle guest-specific logic
                LoadProgress(newUser.UserId);
            }
            else
            {
                Debug.LogError("Guest sign-in failed: " + authTask.Exception);
                loginPanel.SetActive(true);  // Ensure login panel is active in case of failure
            }
        });
    }

    public void SaveProgress(string userId, string progressData)
    {
        Dictionary<string, object> userData = new Dictionary<string, object>
        {
            { "username", username },
            { "email", email },
            { "progress", progressData }
        };

        databaseReference.Child("users").Child(userId).SetValueAsync(userData).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("Progress saved successfully");
            }
            else
            {
                Debug.LogError("Failed to save progress: " + task.Exception);
            }
        });
    }

    public void SaveCurrentProgress()
    {
        string userId = auth.CurrentUser.UserId;
        string progressData = "Save Sign in Data"; // Replace with your actual progress data
        SaveProgress(userId, progressData);
    }

    public void LoadProgress(string userId)
    {
        databaseReference.Child("users").Child(userId).GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                DataSnapshot snapshot = task.Result;
                string progressData = snapshot.Child("progress").Value?.ToString();
                string loadedUsername = snapshot.Child("username").Value?.ToString();
                string loadedEmail = snapshot.Child("email").Value?.ToString();
                
                username = loadedUsername ?? "User";
                email = loadedEmail ?? "";

                userNameText.text = username;
                userEmailText.text = email;

                Debug.Log("Loaded progress: " + progressData);
                // Handle loaded progress data
            }
            else
            {
                Debug.LogError("Failed to load progress: " + task.Exception);
            }
        });
    }

    private void HandleSignedInUser(FirebaseUser user)
    {
        userNameText.text = user.DisplayName ?? "User";
        userEmailText.text = user.Email ?? "";

        // Load the profile picture if available
        if (user.PhotoUrl != null)
        {
            imageURL = user.PhotoUrl.ToString();
            StartCoroutine(LoadProfilePic());
        }

        // Load existing progress or other user-specific data
        LoadProgress(user.UserId);

        loginPanel.SetActive(false);
    }
}
