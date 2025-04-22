using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private GameObject progressBar;
    [SerializeField]
    private Text textPercent;

    private void Start()
    {
        if (!string.IsNullOrEmpty(SceneLoadData.nextSceneName))
            StartCoroutine(LoadSceneAsync(SceneLoadData.nextSceneName));
        else
            Debug.LogError("Chưa đặt tên scene cần load vào SceneLoadData!");
    }

    public IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (fakeProgress < 1f)
        {
            if (fakeProgress < operation.progress)
                fakeProgress += Random.Range(0.01f, 0.05f);
            else
                fakeProgress += Random.Range(0.005f, 0.015f);

            fakeProgress = Mathf.Clamp01(fakeProgress);
            progressBar.GetComponent<Image>().fillAmount = fakeProgress;
            textPercent.text = (fakeProgress * 100).ToString("0") + "%";

            yield return new WaitForSeconds(Random.Range(0.01f, 0.1f));

            if (fakeProgress >= 1f)
                operation.allowSceneActivation = true;
        }
    }
    //public IEnumerator LoadSceneFixedTime(string sceneName)
    //{
    //    float elapsedTime = 0f;

    //    while (elapsedTime < fixedLoadingTime)
    //    {
    //        float progress = Mathf.Clamp01(elapsedTime / fixedLoadingTime);
    //        progressBar.GetComponent<Image>().fillAmount = progress;
    //        textPercent.text = (progress * 100).ToString("0") + "%";
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }
    //    SceneManager.LoadScene(sceneName);
    //}

    //public IEnumerator LoadSceneFixedTime(string sceneName)
    //{
    //    float elapsedTime = 0f;

    //    while (elapsedTime < fixedLoadingTime)
    //    {
    //        // Tính t từ 0 -> 1
    //        float t = Mathf.Clamp01(elapsedTime / fixedLoadingTime);

    //        // Dùng SmoothStep để làm cho tốc độ lúc đầu chậm, giữa nhanh, cuối chậm
    //        float progress = Mathf.SmoothStep(0, 1, t);

    //        progressBar.GetComponent<Image>().fillAmount = progress;
    //        textPercent.text = (progress * 100).ToString("0") + "%";

    //        float speedFactor = Mathf.SmoothStep(0.5f, 1.5f, Mathf.Sin(Time.time * 2f) * 0.5f + 0.5f);
    //        elapsedTime += Time.deltaTime * speedFactor;
    //        yield return null;
    //    }

    //    SceneManager.LoadScene(sceneName);
    //}

}
