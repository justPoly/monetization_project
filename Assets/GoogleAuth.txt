using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Google;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Firebase.Firestore;

public class GoogleAuthentication : MonoBehaviour
{
    public string imageURL, username, email;
    public TMP_Text userNameText, userEmailText;
    public Image profilePic;
    public Sprite defaultProfilePic; // Assign this in the Unity Inspector with a default sprite image
    public GameObject loginPanel;
     public string webClientId = "883892493947-v6s268kg735mqiq2h4s3ik9b3ro51qun.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;
    private FirebaseAuth auth;
    private DatabaseReference databaseReference;
    private FirebaseFirestore firestore;
    private GoogleSignInUser lastGoogleSignInUser;

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
            firestore = FirebaseFirestore.DefaultInstance;

            if (auth.CurrentUser != null)
            {
                HandleSignedInUser(auth.CurrentUser);
            }
            else
            {
                OnGoogleSilentlySignIn();
            }
        });
    }

    void Start()
    {
        if (auth.CurrentUser != null && auth.CurrentUser.ProviderData.Any(provider => provider.ProviderId == "google.com"))
        {
            OnGoogleSilentlySignIn();
            loginPanel.SetActive(false);
        }
    }

    public void OnSignIn()
    {
        GoogleSignIn.Configuration = configuration;
        GoogleSignIn.Configuration.UseGameSignIn = false;
        GoogleSignIn.Configuration.RequestIdToken = true;
        GoogleSignIn.Configuration.RequestEmail = true;

        GoogleSignIn.DefaultInstance.SignIn().ContinueWith(task =>
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

                        SaveUserData(newUser.UserId, task.Result.DisplayName, task.Result.Email, "", "google");
                        SaveUserToken(newUser.UserId, task.Result.IdToken); // Save user token to Firestore
                        HandleSignedInUser(newUser);
                        lastGoogleSignInUser = task.Result;
                    }
                    else
                    {
                        Debug.LogError("Firebase authentication failed: " + authTask.Exception);
                    }
                });

                StartCoroutine(LoadProfilePic());
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void SaveUserData(string userId, string userName, string userEmail, string progressData, string lastSignInMethod)
    {
        var userData = new Dictionary<string, object>
        {
            { "username", userName },
            { "email", userEmail },
            { "progress", progressData },
            { "lastSignInMethod", lastSignInMethod }
        };

        databaseReference.Child("users").Child(userId).SetValueAsync(userData).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("User data saved successfully");
            }
            else
            {
                Debug.LogError("Failed to save user data: " + task.Exception);
            }
        });
    }

    private void SaveUserToken(string userId, string idToken)
    {
        var userTokenData = new Dictionary<string, object>
        {
            { "idToken", idToken }
        };

        firestore.Collection("userTokens").Document(userId).SetAsync(userTokenData).ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                Debug.Log("User token saved successfully");
            }
            else
            {
                Debug.LogError("Failed to save user token: " + task.Exception);
            }
        });
    }

    public void OnGoogleSilentlySignIn()
    {
        if (auth.CurrentUser != null)
        {
            HandleSignedInUser(auth.CurrentUser);
            return;
        }

        string userId = PlayerPrefs.GetString("userId", null);
        if (string.IsNullOrEmpty(userId))
        {
            Debug.Log("No previous Google sign-in data found.");
            return;
        }

        firestore.Collection("userTokens").Document(userId).GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted && !task.IsFaulted && !task.IsCanceled)
            {
                DocumentSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string idToken = snapshot.GetValue<string>("idToken");
                    Firebase.Auth.Credential credential = Firebase.Auth.GoogleAuthProvider.GetCredential(idToken, null);

                    auth.SignInWithCredentialAsync(credential).ContinueWith(authTask =>
                    {
                        if (authTask.IsCompleted)
                        {
                            FirebaseUser newUser = authTask.Result;
                            Debug.Log("User signed in successfully: " + newUser.DisplayName + " (" + newUser.UserId + ")");
                            HandleSignedInUser(newUser);
                        }
                        else
                        {
                            Debug.LogError("Firebase authentication failed: " + authTask.Exception);
                        }
                    });
                }
                else
                {
                    Debug.Log("No token found for user.");
                }
            }
            else
            {
                Debug.LogError("Failed to retrieve user token: " + task.Exception);
            }
        });
    }

    IEnumerator LoadProfilePic()
    {
        using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(imageURL))
        {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to load profile picture: " + uwr.error);
                profilePic.sprite = defaultProfilePic;
            }
            else
            {
                Texture2D texture = DownloadHandlerTexture.GetContent(uwr);
                profilePic.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
    }

    public void OnGoogleSignOut()
    {
        ResetUI();
        GoogleSignIn.DefaultInstance.SignOut();
        auth.SignOut();
    }

    public void OnGuestSignOut()
    {
        ResetUI();
        auth.SignOut();
    }

    private void ResetUI()
    {
        userNameText.text = "";
        userEmailText.text = "";
        profilePic.sprite = defaultProfilePic;
        loginPanel.SetActive(true);
    }

    public void OnSignOut()
    {
        if (auth.CurrentUser == null)
        {
            return;
        }

        bool isGoogleUser = !auth.CurrentUser.IsAnonymous && auth.CurrentUser.ProviderData.Any(provider => provider.ProviderId == "google.com");
        bool isGuestUser = auth.CurrentUser.IsAnonymous;

        if (isGoogleUser)
        {
            OnGoogleSignOut();
        }
        if (isGuestUser)
        {
            OnGuestSignOut();
        }
    }

    public void SaveProgress(string userId, string progressData)
    {
        var userData = new Dictionary<string, object>
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
                username = snapshot.Child("username").Value?.ToString() ?? "Guest";
                email = snapshot.Child("email").Value?.ToString() ?? "";

                userNameText.text = username;
                userEmailText.text = email;

                Debug.Log("Loaded progress: " + progressData);
            }
            else
            {
                Debug.LogError("Failed to load progress: " + task.Exception);
            }
        });
    }

    private void HandleSignedInUser(FirebaseUser user)
    {
        username = user.DisplayName ?? "Guest";
        email = user.Email ?? "";
        userNameText.text = username;
        userEmailText.text = email;

        if (user.PhotoUrl != null)
        {
            imageURL = user.PhotoUrl.ToString();
            StartCoroutine(LoadProfilePic());
        }
        else
        {
            profilePic.sprite = defaultProfilePic; // Use default profile picture if no PhotoUrl
        }

        LoadProgress(user.UserId);
        loginPanel.SetActive(false);
    }

    public void OnGuestSignIn()
    {
        auth.SignInAnonymouslyAsync().ContinueWith(authTask =>
        {
            if (authTask.IsCompleted && !authTask.IsFaulted)
            {
                FirebaseUser newUser = authTask.Result.User;
                Debug.Log("Guest signed in successfully: " + newUser.UserId);
                username = "Guest";
                email = "";
                userNameText.text = username;
                userEmailText.text = email;
                profilePic.sprite = defaultProfilePic;

                LoadProgress(newUser.UserId);
                loginPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("Guest sign-in failed: " + authTask.Exception);
                loginPanel.SetActive(true);
            }
        });
    }
}
