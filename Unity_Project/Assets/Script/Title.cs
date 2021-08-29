using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Title : MonoBehaviour
{

    [SerializeField]
    private SaveNLoad saveNLoad;



    public void ClickStart()
    {
        SceneManager.LoadScene("Stage1");
        saveNLoad.Save();
        Destroy(gameObject);

    }

    public void Load()
    {
        saveNLoad.Load();
    }

    IEnumerator LoadCoroutine()
    {
        gameObject.SetActive(false);
        yield return null;
    }

    public void Exit()
    {
        Application.Quit();
    }

}
