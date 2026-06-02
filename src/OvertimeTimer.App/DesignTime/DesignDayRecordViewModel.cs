namespace OvertimeTimer.App.DesignTime;

public sealed class DesignDayRecordViewModel
{
    public string DateDisplay { get; } = "2026-06-02";

    public int OvertimeHours { get; } = 2;

    public int OvertimeMinutes { get; } = 30;

    public string DiaryMarkdown { get; } = "今天完成了设计时数据接线。";

    public string DiaryPreview { get; } = "今天完成了设计时数据接线。";

    public object SaveCommand { get; } = new object();
}
