using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManage : MonoBehaviour
{
    public GameObject gameOverUI;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -10f)
        {
            GameOver();
        }
    }

    #region Fallend Methods
    private void GameOver()
    {
        gameOverUI.SetActive(true); // 게임오버 UI를 활성화합니다.

        if (Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }

        //Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EnterScene"); // 엔터 씬 호출
        //Time.timeScale = 1f; // 게임 시간을 다시 시작합니다.
    }
    #endregion
}
