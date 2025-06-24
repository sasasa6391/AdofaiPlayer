using DG.Tweening;
using EasingCore;
using FancyScrollView;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class SongData
{
    //public string Message { get; }

    public string title;
    public string artist;
    public string author;
    public string desc;
    public int difficulty;
    public string fileName;
    public string previewImage;
    public string songFileName;
    public SongData(string title, string artist, string author, string desc, int difficulty, string fileName, string previewImage, string songFileName)
    {
        this.title = title;
        this.artist = artist;
        this.author = author;
        this.desc = desc;
        this.difficulty = difficulty;
        this.fileName = fileName;
        this.previewImage = previewImage;
        this.songFileName = songFileName;
    }
}
public class Context
{
    public int SelectedIndex = -1;
    public Action<int> OnCellClicked;
}

public class MainScrollView : FancyScrollView<SongData, Context>, IBeginDragHandler, IEndDragHandler
{

    [SerializeField] FancyScrollView.Scroller scroller = default;
    [SerializeField] GameObject cellPrefab = default;

    public CanvasGroup songInfoGroup;

    Action<int> onSelectionChanged;
    protected override GameObject CellPrefab => cellPrefab;
    protected override void Initialize()
    {
        base.Initialize();

        Context.OnCellClicked = SelectCell;

        scroller.OnValueChanged(UpdatePosition);
        scroller.OnSelectionChanged(UpdateSelection);
    }

    public void Update()
    {
    }


    void UpdateSelection(int index)
    {
        if (Context.SelectedIndex == index)
        {
            return;
        }

        Context.SelectedIndex = index;
        Refresh();

        onSelectionChanged?.Invoke(index);
    }

    public void UpdateData(IList<SongData> items)
    {
        UpdateContents(items);
        scroller.SetTotalCount(items.Count);
    }

    public void OnSelectionChanged(Action<int> callback)
    {
        onSelectionChanged = callback;
    }

    public void SelectNextCell()
    {
        SelectCell(Context.SelectedIndex + 1);
    }

    public void SelectPrevCell()
    {
        SelectCell(Context.SelectedIndex - 1);
    }

    public void SelectCell(int index)
    {
        if (index < 0 || index >= ItemsSource.Count || index == Context.SelectedIndex)
        {
            return;
        }

        UpdateSelection(index);
        
        scroller.ScrollTo(index, 0.0f);
    }


    private Tweener _beginTween;
    private Tweener _endTween;
    public void OnBeginDrag(PointerEventData eventData)
    {
        _beginTween?.Kill();
        _endTween?.Kill();
        _beginTween = songInfoGroup.DOFade(0.0f, 0.3f);
        songInfoGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _endTween?.Kill();
        _endTween = songInfoGroup.DOFade(1.0f, 0.3f).SetDelay(0.6f);
        songInfoGroup.blocksRaycasts = true;
    }


}

