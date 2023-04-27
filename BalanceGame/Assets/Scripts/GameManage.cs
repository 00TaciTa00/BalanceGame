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
        gameOverUI.SetActive(true); // ���ӿ��� UI�� Ȱ��ȭ�մϴ�.

        if (Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }

        //Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("EnterScene"); // ���� �� ȣ��
        //Time.timeScale = 1f; // ���� �ð��� �ٽ� �����մϴ�.
    }
    #endregion
}
