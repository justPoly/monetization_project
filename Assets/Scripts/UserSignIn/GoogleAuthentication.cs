using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Google;
using UnityEngine.UI;
using TMPro;

public class GoogleAuthentication : MonoBehaviour
{
    public string imageURL, username, email;

    public TMP_Text userNameText, userEmailText;

    public Image profilePic;

    public GameObject loginPanel, profilePanel;

    public string webClientId = "883892493947-v6s268kg735mqiq2h4s3ik9b3ro51qun.apps.googleusercontent.com";

    private GoogleSignInConfiguration configuration;

    // Defer the configuration creation until Awake so the web Client ID
    // Can be set via the property inspector in the Editor.
    void Awake() {
      configuration = new GoogleSignInConfiguration {
            WebClientId = webClientId,
            RequestIdToken = true
      };
    }

    public void OnSignIn() {
      GoogleSignIn.Configuration = configuration;
      GoogleSignIn.Configuration.UseGameSignIn = false;
      GoogleSignIn.Configuration.RequestIdToken = true;
      GoogleSignIn.Configuration.RequestEmail = true;

      GoogleSignIn.DefaultInstance.SignIn().ContinueWith(
        OnAuthenticationFinished, TaskScheduler.Default);
    }

    internal void OnAuthenticationFinished(Task<GoogleSignInUser> task) {
      if (task.IsFaulted) {
        using (IEnumerator<System.Exception> enumerator =
                task.Exception.InnerExceptions.GetEnumerator()) {
          if (enumerator.MoveNext()) {
            GoogleSignIn.SignInException error =
                    (GoogleSignIn.SignInException)enumerator.Current;
                    Debug.LogError("Got Error: " + error.Status + " " + error.Message);
          } else {
                    Debug.LogError("Got Unexpected Exception?!?" + task.Exception);
          }
        }
      } else if(task.IsCanceled) {
                Debug.LogError("Canceled");
      } else  {
                Debug.LogError("Welcome: " + task.Result.DisplayName + "!");

                userNameText.text = "" + task.Result.DisplayName;
                userEmailText.text = "" + task.Result.Email;

                imageURL = task.Result.ImageUrl.ToString();
                loginPanel.SetActive(false);
                profilePanel.SetActive(true);
                StartCoroutine(LoadProfilePic());
      }
    }

    IEnumerator LoadProfilePic()
    {
        WWW www = new WWW(imageURL);
        yield return www;

        profilePic.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0,0));
    }

    public void OnSignOut() 
    {
        userNameText.text = "";
        userEmailText.text = "";

        imageURL = "";
        loginPanel.SetActive(true);
        profilePanel.SetActive(false);
        Debug.LogError("Calling SignOut");
        GoogleSignIn.DefaultInstance.SignOut();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
