using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //점수 및 스테이지 관리
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;

    public int health;

    public PlayerMove player;
    public GameObject[] stages;

    public Image[] healthImage;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartButton;

    void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void NextStage()
    {
        if (stageIndex < stages.Length-1)
        {
            stages[stageIndex].SetActive(false);
            stageIndex++;
            stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = $"STAGE {stageIndex+1}";  
        }
        else
        {
            Time.timeScale = 0;
            Debug.Log("게임 클리어!");
            Text btnText = RestartButton.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            RestartButton.SetActive(true);
        }
        

        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        if (health > 1)
        {
            health--;
            healthImage[health].color = new Color(1,1,1,0.4f);
        }
        else
        {
            player.OnDie();
            RestartButton.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //plaer reposition
            if (health > 1)
            {
                PlayerReposition();
            }

            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(-6,3,0);
        player.VelocityZero(); 
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
    }
}
