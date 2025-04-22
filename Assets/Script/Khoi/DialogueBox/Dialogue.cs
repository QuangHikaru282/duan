using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public string[] lines;
    public float textSpeed;
    [SerializeField]
    private SceneSwitcher sceneSwitcher;
    [SerializeField]
    private GameObject Decorate;

    private int index;

    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            Animator animator = sceneSwitcher.animatorFire;
            
            animator.SetBool("Fire", true);
            StartCoroutine(WaitForAnimation(animator));
        }

    }
    

    private IEnumerator WaitForAnimation(Animator animator)
    {
        // Đợi cho đến khi animation kết thúc
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        // Thực hiện các hành động sau khi đợi
        GameObject movingObject = sceneSwitcher.movingObject;
        Destroy(movingObject);
        yield return new WaitForSeconds(0.3f);
        Destroy(Decorate);

        gameObject.SetActive(false);
        // Dừng animation hoặc chuyển sang trạng thái khác sau khi animation hoàn thành
    }
}

