# 数据模型

## 1. 日常记录

```csharp
class DailyRecord
{
    public DateOnly Date { get; set; }
    public int OvertimeHours { get; set; }
    public int OvertimeMinutes { get; set; }
    public string DiaryMarkdown { get; set; } = "";
    public DateTime LastModified { get; set; }
}
```

## 2. 工作日配置

```csharp
enum WorkScheduleMode
{
    Weekly,
    Daily
}

class WorkScheduleConfig
{
    public WorkScheduleMode Mode { get; set; }
    public DateOnly AnchorDate { get; set; }
    public int WeekCycleCount { get; set; } = 1;
    public int CurrentCycleWeekIndex { get; set; } = 1;
    public List<WeeklyCycleItem> WeeklyCycles { get; set; } = new();
    public int WorkDays { get; set; }
    public int RestDays { get; set; }
    public int AnchorWorkDayIndex { get; set; } = 1;
}

class WeeklyCycleItem
{
    public int WeekIndex { get; set; }
    public bool MondayWork { get; set; }
    public bool TuesdayWork { get; set; }
    public bool WednesdayWork { get; set; }
    public bool ThursdayWork { get; set; }
    public bool FridayWork { get; set; }
    public bool SaturdayWork { get; set; }
    public bool SundayWork { get; set; }
}
```

## 3. 本地持久化根对象

```csharp
class AppDataStore
{
    public WorkScheduleConfig WorkScheduleConfig { get; set; } = new();
    public List<DailyRecord> DailyRecords { get; set; } = new();
    public DateOnly? LastSelectedDate { get; set; }
}
```

## 4. 说明

- `DailyRecord` 以日期为唯一主键。
- `WorkScheduleConfig` 保存排班模式和周期参数。
- `AppDataStore` 用于统一承载本地缓存读取和写入的数据。
