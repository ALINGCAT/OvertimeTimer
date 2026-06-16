using OvertimeTimer.App.Localization;
using OvertimeTimer.App.Models;

namespace OvertimeTimer.App.ViewModels;

public sealed class WeeklyCycleItemViewModel : ViewModelBase
{
    public WeeklyCycleItemViewModel(int weekIndex)
    {
        WeekIndex = weekIndex;
        LocalizationService.Instance.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == "Item[]")
            {
                RaisePropertyChanged(nameof(WeekLabel));
            }
        };
    }

    public int WeekIndex { get; set; }

    public string WeekLabel => string.Format(LocalizationService.Instance["Calendar.WeekLabelFormat"], WeekIndex);

    public bool MondayWork { get; set; } = true;

    public bool TuesdayWork { get; set; } = true;

    public bool WednesdayWork { get; set; } = true;

    public bool ThursdayWork { get; set; } = true;

    public bool FridayWork { get; set; } = true;

    public bool SaturdayWork { get; set; }

    public bool SundayWork { get; set; }

    public static WeeklyCycleItemViewModel FromModel(WeeklyCycleItem model)
    {
        return new WeeklyCycleItemViewModel(model.WeekIndex)
        {
            MondayWork = model.MondayWork,
            TuesdayWork = model.TuesdayWork,
            WednesdayWork = model.WednesdayWork,
            ThursdayWork = model.ThursdayWork,
            FridayWork = model.FridayWork,
            SaturdayWork = model.SaturdayWork,
            SundayWork = model.SundayWork
        };
    }

    public WeeklyCycleItem ToModel()
    {
        return new WeeklyCycleItem
        {
            WeekIndex = WeekIndex,
            MondayWork = MondayWork,
            TuesdayWork = TuesdayWork,
            WednesdayWork = WednesdayWork,
            ThursdayWork = ThursdayWork,
            FridayWork = FridayWork,
            SaturdayWork = SaturdayWork,
            SundayWork = SundayWork
        };
    }
}
