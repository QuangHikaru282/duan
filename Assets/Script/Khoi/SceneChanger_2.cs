using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger_2 : MonoBehaviour
{
    //[SerializeField]
    public static string NEXT_SCENE = "scene2";
    
    private float fixedLoadingTime = 3f;

    [SerializeField]
    private GameObject outro;
    [SerializeField]
    private Animator outroAnimator;
    [SerializeField]
    private float transitionTime = 1f; // Thời gian chạy animation

    private void Start()
    {
        StartCoroutine(LoadSceneFixedTime("scene2"));
    }

    public IEnumerator LoadSceneFixedTime(string sceneName)
    {
        // Chạy animation outro
        if (outroAnimator != null)
        {
            outro.SetActive(true);
            outroAnimator.SetBool("isOutro", true);
        }

        // Chờ cho animation chạy xong
        yield return new WaitForSeconds(transitionTime);
        outro.SetActive(false);
        // Chuyển scene
        SceneManager.LoadScene(sceneName);
    }
}
