using FancyScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainUICell : FancyCell<SongData, Context>
{
    [SerializeField] Animator animator = default;
    [SerializeField] Image image = default;

    static class AnimatorHash
    {
        public static readonly int Scroll = Animator.StringToHash("MainUICell");
    }

    public override void Initialize()
    {
        //button.onClick.AddListener(() => Context.OnCellClicked?.Invoke(Index));
    }

    public override void UpdateContent(SongData songData)
    {
        image.sprite = ResourceManager.LoadResourceSprite($"{songData.fileName}/{songData.previewImage}");
    }

    public override void UpdatePosition(float position)
    {
        currentPosition = position;

        if (animator.isActiveAndEnabled)
        {
            animator.Play(AnimatorHash.Scroll, -1, position);
        }

        animator.speed = 0;
    }

    float currentPosition = 0;

    void OnEnable() => UpdatePosition(currentPosition);

}
