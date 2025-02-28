using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger_2 : MonoBehaviour
{
    public static string NEXT_SCENE = "scene2";
    [SerializeField] private GameObject outro;
    [SerializeField] private Animator outroAnimator;

    private void Start()
    {
        StartCoroutine(LoadSceneWhenAnimationEnds("scene2"));
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ShowIntroAnimation());
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public IEnumerator LoadSceneWhenAnimationEnds(string sceneName)
    {
        if (outroAnimator != null)
        {
            outro.SetActive(true);
            outroAnimator.SetBool("isOutro", true);
            

            // Chờ animation hoàn thành
            yield return new WaitUntil(() => outroAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
            outroAnimator.SetBool("isOutro", false);
            outro.SetActive(false);
            SceneManager.LoadScene(sceneName);
        }
    }
    private IEnumerator ShowIntroAnimation()
    {
        if (outro == null || outroAnimator == null)
        {
            Debug.LogError("Outro hoặc OutroAnimator chưa được thiết lập trong scene mới!");
            yield break;
        }

        outro.SetActive(true);
        outroAnimator.SetBool("isIntro", true);

        yield return new WaitUntil(() => outroAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        outro.SetActive(false);
        Destroy(gameObject);
    }
}
