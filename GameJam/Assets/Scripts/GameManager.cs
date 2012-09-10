using UnityEngine;
using System.Collections;

#region Enums
public enum GameState
{
    OpeningWindow = 1,
    PlayGame = 2,
    GameOver = 3,
    Pause = 4
}
public  enum CurrentPlayMode
{
    Black = 1,
    White = 2,
    Grey = 3
}
#endregion
public class GameManager : MonoBehaviour
{

    #region Variables

    public GameObject OpeningWindow, GameOverWindow, SelectionWindow,
                    BlackWorld, WhiteWorld,
                    BlackCam, WhiteCam, MainCam,
                    Facebook, fbbutton, fbsuccess,
                    Character;
    UILabel fb;
    public Material BlackMat, WhiteMat, CharMat;
    public Texture2D blackTexture, whiteTexture;
    public static GameState gameState = GameState.OpeningWindow;
    public static CurrentPlayMode currentPlayMode = CurrentPlayMode.Black;
    private bool toggle;
    public bool cheats;
    private int deaths;
    private float time, maxTime;

    #endregion

    #region Mono Inherit Functions

    void Awake () {
        BlackMat.color = new Color(BlackMat.color.r, BlackMat.color.g, BlackMat.color.b, 1f);
        WhiteMat.color = new Color(WhiteMat.color.r, WhiteMat.color.g, WhiteMat.color.b, 1f);
        gameState = GameState.OpeningWindow;
        OpeningWindow.SetActiveRecursively(true);
        GameOverWindow.SetActiveRecursively(false);
        SelectionWindow.SetActiveRecursively(false);
        

        BlackCam.camera.rect = new Rect(0.5f, 0, 0.5f, 1);
        WhiteCam.camera.rect = new Rect(0, 0, 0.5f, 1);

        MainCam.camera.rect = new Rect(0, 0, 1, 1);
        MainCam.active = Facebook.active = true;
        BlackCam.active = WhiteCam.active = fbsuccess.active = false;
        fb = fbsuccess.GetComponent<UILabel>();

	
	}
	
	void Update ()
    {

        #region Fade in/out
        if (toggle)
        {
            if (GameManager.currentPlayMode == CurrentPlayMode.Black)
            {
                if (Toggle(ref WhiteMat, true)&&Toggle(ref BlackMat, false))
                {
                    toggle = false;
                    ActivateBlackMode(false);
                    GameManager.currentPlayMode = CurrentPlayMode.White;
                }
            }
            else if (GameManager.currentPlayMode == CurrentPlayMode.White)
            {
                if(Toggle(ref BlackMat, true)&& Toggle(ref WhiteMat, false))
                {
                    toggle = false;
                    ActivateWhiteMode(false);
                    GameManager.currentPlayMode = CurrentPlayMode.Black;
                }
            }
        }
        #endregion
    }

    #endregion

    #region UI Events
    void Switch()
    {
        if (currentPlayMode == CurrentPlayMode.Grey)
            return;

        toggle = true;
        switch (GameManager.currentPlayMode)
        {
            case CurrentPlayMode.Black:
                ActivateWhiteMode(true);
                WhiteMat.color = new Color(WhiteMat.color.r, WhiteMat.color.g, WhiteMat.color.b, 0);
                break;
            case CurrentPlayMode.White:
                ActivateBlackMode(true);
                BlackMat.color = new Color(BlackMat.color.r, BlackMat.color.g, BlackMat.color.b, 0);
                break;
        }
    }
    void Play()
    {
        OpeningWindow.SetActiveRecursively( false );
        SelectionWindow.SetActiveRecursively(true);
    }
    void Gray() 
    {
        WhiteCam.active = BlackCam.active = true;
        SelectionWindow.SetActiveRecursively(false);
        gameState = GameState.PlayGame;
        currentPlayMode = CurrentPlayMode.Grey;
        time = Time.timeSinceLevelLoad;
    }
    void Black() 
    {
        WhiteCam.active = BlackCam.active = false;
        MainCam.active = true;
        ActivateWhiteMode(false);
        ActivateBlackMode(true);
        SelectionWindow.SetActiveRecursively(false);
        gameState = GameState.PlayGame;
        GameManager.currentPlayMode = CurrentPlayMode.Black;
        time = Time.timeSinceLevelLoad;
    }
    void White() 
    {
        WhiteCam.active = BlackCam.active = false;
        MainCam.active = true;
        ActivateBlackMode(false);
        ActivateWhiteMode(true);
        SelectionWindow.SetActiveRecursively(false);
        gameState = GameState.PlayGame;
        GameManager.currentPlayMode = CurrentPlayMode.White;
        time = Time.timeSinceLevelLoad;
    }
    void FaceBook()
    {
        Facebook.SendMessage("GetToken");
    }
    void PostResults()
    {
        float timer = Time.timeSinceLevelLoad - time;
        if (timer < maxTime)
            timer = maxTime;
        else
            maxTime = timer;
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        Facebook.GetComponent<Facebook>().Publish("I died "+deaths+" times and lasted for  a maximum of " + minutes + " minutes " + seconds + " seconds!"  );
    }
    void SuccessFacebookLink()
    {
        if (fb.text != "Success!")
        {
            fb.color = Color.green;
            fb.text = "Success!";
        }
    }
    void ProcessFacebookLink()
    {
        if (fbbutton.active && fb.text != "Processing... Please Wait")
        {
            fbsuccess.active = true;
            fb.color = Color.yellow;
            fb.text = "Processing... Please Wait";
        }
        fbbutton.SetActiveRecursively(false);
    }
    void FailedFacebookLink()
    {
        if (fb.text != "There seems to have been a problem\nWe are having issues with the web version.\nSorry about that, feel free to play!")
        {
            fb.color = Color.red;
            fb.text = "There seems to have been a problem\nWe are having issues with the web version.\nSorry about that, feel free to play!";
        }
    }
    void Pause()
    {
        gameState = GameState.Pause;
    }
    void GameOver()
    {
        deaths++;
        float timer = Time.timeSinceLevelLoad - time;
        if (timer < maxTime)
            timer = maxTime;
        else
            maxTime = timer;
        string minutes = Mathf.Floor(timer / 60).ToString("00");
        string seconds = (timer % 60).ToString("00");
        gameState = GameState.GameOver;
        OpeningWindow.SetActiveRecursively(false);
        GameOverWindow.SetActiveRecursively(true);
        if (fb.text == "There seems to have been a problem\nWe are having issues with the web version.\nSorry about that, feel free to play!")
            GameObject.Find("btnFBResults").SetActiveRecursively(false);
        UILabel deathLbl = GameObject.Find("lblDeaths").GetComponent<UILabel>();
        UILabel timeLbl = GameObject.Find("lblTime").GetComponent<UILabel>();
        deathLbl.text = deathLbl.text.Replace("0", deaths.ToString());
        timeLbl.text = timeLbl.text.Replace("0", minutes + " minutes " + seconds + " seconds");
    }
    void Replay()
    {

        gameState = GameState.PlayGame;
        time = Time.timeSinceLevelLoad;
        UILabel deathLbl = GameObject.Find("lblDeaths").GetComponent<UILabel>();
        UILabel timeLbl = GameObject.Find("lblTime").GetComponent<UILabel>();
        deathLbl.text = "You died: 0 time(s)!";
        timeLbl.text = "Your max time: 0";
        Character.SetActiveRecursively(true);
        GameOverWindow.SetActiveRecursively(false);
    }
    #endregion

    #region Utilities

    bool Toggle(ref Material mat, bool pulse)
    {
        if ((mat.color.a <= 0 && !pulse) || (mat.color.a >= 1 && pulse))
            return true;

        mat.color = new Color(mat.color.r, mat.color.g, mat.color.b, (pulse) ? (mat.color.a + .1f) : (mat.color.a - .1f));

        return false;
    }

    void ActivateBlackMode(bool active)
    {
        BlackWorld.SetActiveRecursively(active);
        if (active)
        {
            MainCam.camera.backgroundColor = new Color(.85f, .85f, .85f);
            CharMat.mainTexture = blackTexture;
        }
        
    }
    void ActivateWhiteMode(bool active)
    {
        WhiteWorld.SetActiveRecursively(active);
        if (active)
        {
            MainCam.camera.backgroundColor = new Color(.29f, .29f, .29f);
            CharMat.mainTexture = whiteTexture;
        }
    }

    void OnApplicationQuit()
    {
        BlackMat.color = new Color(BlackMat.color.r, BlackMat.color.g, BlackMat.color.b, 1);
        WhiteMat.color = new Color(WhiteMat.color.r, WhiteMat.color.g, WhiteMat.color.b, 1);
    }

    #endregion


}
