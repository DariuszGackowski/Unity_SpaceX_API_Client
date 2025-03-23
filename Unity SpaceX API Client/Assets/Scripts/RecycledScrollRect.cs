using SpaceXLaunchesBrowser;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RecycledScrollRect : ScrollRect
{
    public RectTransform ContentOffsetRectTransform;
    public int SelectionRange = 5;

    private int _currentMiddleIndex;
    protected override void Start()
    {
        base.Start();

        onValueChanged.AddListener(CheckScrollPosition);
    }
    protected override void SetContentAnchoredPosition(Vector2 position) 
    {
        if (!horizontal)
            position.x = content.anchoredPosition.x;
        if (!vertical)
            position.y = content.anchoredPosition.y;

        if (position != content.anchoredPosition)
        {
            content.anchoredPosition = position;
            ContentOffsetRectTransform.anchoredPosition = -content.anchoredPosition;

            UpdateBounds();
        }
    }
    private void CheckScrollPosition(Vector2 scrollPosition)
    {
        CheckLaunchItemVisibility();
    }
    private void CheckLaunchItemVisibility()
    {
        float launchItemHeight = SpaceXLaunchesBrowserCanvasManager.LaunchItems[0].GetComponent<RectTransform>().sizeDelta.y;
        int launchItemOffestInCount = (int)(content.localPosition.y / launchItemHeight);

        Debug.Log("(int)(LaunchItemContent.position.y " + (int)(content.localPosition.y));
        Debug.Log("launchItemHeight " + launchItemHeight);
        Debug.Log("launchItemOffestInCount " + launchItemOffestInCount);

        int newMiddleIndex = SelectionRange + launchItemOffestInCount;

        newMiddleIndex = newMiddleIndex <= SelectionRange ? SelectionRange : newMiddleIndex;
        newMiddleIndex = newMiddleIndex >= SpaceXLaunchesBrowserCanvasManager.LaunchItems.Count - 1 - SelectionRange ? SpaceXLaunchesBrowserCanvasManager.LaunchItems.Count - 1 - SelectionRange : newMiddleIndex;
        if (_currentMiddleIndex == newMiddleIndex) return;

        int higherIndex = newMiddleIndex - SelectionRange < 0 ? 0 : newMiddleIndex - SelectionRange;
        int lowerIndex = newMiddleIndex + SelectionRange > SpaceXLaunchesBrowserCanvasManager.LaunchItems.Count - 1 ? SpaceXLaunchesBrowserCanvasManager.LaunchItems.Count - 1 : newMiddleIndex + SelectionRange;

        Debug.Log("middleIndex " + newMiddleIndex);
        Debug.Log("higherIndex " + higherIndex);
        Debug.Log("lowerIndex " + lowerIndex);

        foreach (LaunchItem launchItem in SpaceXLaunchesBrowserCanvasManager.LaunchItems)
        {
            if (!launchItem.ActiveSelf && launchItem.Identifier >= higherIndex && launchItem.Identifier <= lowerIndex)
            {
                launchItem.SetActive();
            }
            else if (launchItem.ActiveSelf && (launchItem.Identifier < higherIndex || launchItem.Identifier > lowerIndex))
            {
                launchItem.SetInactive();
            }
        }
    }
}
