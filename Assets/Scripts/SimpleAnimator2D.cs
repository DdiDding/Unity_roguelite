using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimator2D : MonoBehaviour
{
    public enum EAnimationType
    {
        Loop = 0,
        Once = 1,
        OnceAndDisable = 2
    }

    public EAnimationType animationType;

    public static List<SimpleAnimator2D> activeInstances = new List<SimpleAnimator2D>();

    public SpriteRenderer[] renderers = new SpriteRenderer[0];

    public Sprite[] sprites = new Sprite[0];

    public Timer animationTimer;
    public float animationTime = 0.05f;

    private int currentIdx;

    public bool randomStartIdx;

    [Space(10f)]
    public bool ignoreCulling;

    private bool endAnimation;

    private void Awake()
    {
        animationTimer = new Timer(animationTime);
    }
    private void OnEnable()
    {
        activeInstances.Add(this);
    }

    private void OnDisable()
    {
        activeInstances.Remove(this);
    }

    private void Start()
    {
        if (randomStartIdx)
        {
            currentIdx = Random.Range(0, sprites.Length);
        }
    }

    public void Update()
    {
        if (!Application.isPlaying)
            return;

        animationTimer.AccrueTime(Time.deltaTime);
        if (animationTimer.IsElapsed() == false)
        {
            return;
        }

        currentIdx++;
        if (currentIdx >= sprites.Length)
        {
            if (animationType == EAnimationType.Once)
            {
                currentIdx = sprites.Length - 1;
            }
            else if (animationType == EAnimationType.OnceAndDisable)
            {
                currentIdx = sprites.Length - 1;
                endAnimation = true;
            }
            else if (animationType == EAnimationType.Loop)
            {
                currentIdx = 0;
            }
        }
        SpriteRenderer[] array = renderers;
        for (int i = 0; i < array.Length; i++)
        {
            array[i].sprite = sprites[currentIdx];
        }
    }

    private void LateUpdate()
    {
        if (endAnimation)
        {
            endAnimation = false;
            base.gameObject.SetActive(value: false);
        }
    }

    public void ResetIdx()
    {
        endAnimation = false;
        currentIdx = 0;
    }
}
