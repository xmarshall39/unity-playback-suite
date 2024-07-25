using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PBExitButton : MonoBehaviour
{
    public void OnClick()
    {
        //Load the playback loading scene
        //Well, we should eventually have a confirmation window
        SceneManager.LoadScene(UPBS.Constants.EDITOR_PLAYBACK_INIT_SCENE_NAME);
    }

}
