using System;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityGameFramework.Runtime;

public readonly struct RunResult
{
    public readonly bool Cleared;
    public readonly TimeSpan Elapsed;
    public readonly string Location;
    public readonly int SapphireEarned;
    public readonly int SapphireTotal;
    public readonly int? HardModePoints;
    public readonly Action ReturnToTown;

    public RunResult(bool cleared, TimeSpan elapsed, string location, int sapphireEarned,
        int sapphireTotal, Action returnToTown = null, int? hardModePoints = null)
    {
        Cleared = cleared;
        Elapsed = elapsed;
        Location = location;
        SapphireEarned = sapphireEarned;
        SapphireTotal = sapphireTotal;
        ReturnToTown = returnToTown;
        HardModePoints = hardModePoints;
    }
}

public sealed class RunResultForm : UIFormLogic
{
    public const string AssetPath = "Assets/Prefabs/UI/RunResultForm.prefab";

    [SerializeField] private TMP_Text outcomeLabel;
    [SerializeField] private Image outcomeAccent;
    [SerializeField] private GameObject victoryPortrait;
    [SerializeField] private GameObject defeatPortrait;
    [SerializeField] private Color victoryColor = new Color32(134, 224, 176, 255);
    [SerializeField] private Color defeatColor = new Color32(255, 152, 145, 255);
    [SerializeField] private TMP_Text elapsedLabel;
    [SerializeField] private TMP_Text locationLabel;
    [SerializeField] private TMP_Text sapphireEarnedLabel;
    [SerializeField] private TMP_Text sapphireTotalLabel;
    [SerializeField] private GameObject hardModeRow;
    [SerializeField] private TMP_Text hardModePointsLabel;
    [SerializeField] private Button returnButton;

    private Action returnToTown;

    private void Awake()
    {
        returnButton.onClick.AddListener(Return);
    }

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        Show(userData is RunResult result ? result : default);
    }

    protected override void OnClose(bool isShutdown, object userData)
    {
        returnToTown = null;
        base.OnClose(isShutdown, userData);
    }

    public void Show(RunResult result)
    {
        var outcomeColor = result.Cleared ? victoryColor : defeatColor;
        outcomeLabel.text = result.Cleared ? "던전 클리어" : "원정 실패";
        outcomeLabel.color = outcomeColor;
        outcomeAccent.color = outcomeColor;
        victoryPortrait.SetActive(result.Cleared);
        defeatPortrait.SetActive(!result.Cleared);

        var elapsed = result.Elapsed < TimeSpan.Zero ? TimeSpan.Zero : result.Elapsed;
        elapsedLabel.text = FormattableString.Invariant($"{(int)elapsed.TotalHours:00}:{elapsed.Minutes:00}:{elapsed.Seconds:00}");
        locationLabel.text = string.IsNullOrWhiteSpace(result.Location) ? "미기록" : result.Location;
        sapphireEarnedLabel.text = result.SapphireEarned.ToString("+0;-0;+0", CultureInfo.InvariantCulture);
        sapphireTotalLabel.text = result.SapphireTotal.ToString(CultureInfo.InvariantCulture);
        hardModeRow.SetActive(result.HardModePoints.HasValue);
        hardModePointsLabel.text = result.HardModePoints.GetValueOrDefault().ToString(CultureInfo.InvariantCulture);

        returnToTown = result.ReturnToTown;
        returnButton.interactable = true;
        gameObject.SetActive(true);
    }

    private void Return()
    {
        if (!returnButton.interactable)
            return;

        returnButton.interactable = false;
        var callback = returnToTown;
        returnToTown = null;

        if (Available)
            GameEntry.GetComponent<UIComponent>().CloseUIForm(UIForm);
        else
            gameObject.SetActive(false);

        callback?.Invoke();
    }
}
