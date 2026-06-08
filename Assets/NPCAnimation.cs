using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine("WaveRoutine");
    }
    IEnumerator WaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(2f);
            animator.SetFloat("WavingAmount", 1f);
            yield return new WaitForSeconds(2f);
            animator.SetFloat("WavingAmount", 0f);
        }
    }
}
