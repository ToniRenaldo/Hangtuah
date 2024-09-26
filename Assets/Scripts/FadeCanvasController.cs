using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FadeCanvasController : MonoBehaviour
{
    public static FadeCanvasController instance;
    public CanvasGroup canvasGroup;

    public float fadeTime;


    private void Awake()
    {
        instance = this;
    }
    Coroutine CR_Fade;

    //FADE OUT -> MENGGELAP
    // FADE IN -> MENERANG
    public async Task FadeIn()
    {
        var currentTime = fadeTime;
        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            canvasGroup.alpha = currentTime / fadeTime;
            await Task.Yield();
        }
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    public async Task FadeOut()
    {
        canvasGroup.blocksRaycasts = true;
        float currentTime = 0;
        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;


            canvasGroup.alpha = currentTime / fadeTime;
            await Task.Yield();
        }
        canvasGroup.alpha = 1;
    }
}
