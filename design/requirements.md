# WPF 加班记录与日历应用需求文档

## 1. 项目目标

实现一个 WPF 桌面应用，用于按日期记录加班时长和日记，并结合用户可配置的工作日规则，在日历中直观展示工作日、休息日、加班标记和日记标记。

## 2. 核心功能

### 2.1 日历

- 首页展示月历。
- 默认选中当天。
- 用户可点击任意日期进行切换。
- 日期上支持状态提示：
  - 有加班记录的日期显示红点。
  - 有日记记录的日期显示绿点。
  - 休息日以浅色样式显示。

### 2.2 当日记录面板

选中某一天后，右侧或下方显示该日编辑面板，包含：

- 加班时长输入：
  - 小时输入框。
  - 分钟输入框。
  - 两个输入框都只能输入数字。
  - 不允许输入 `.`、`-`、空格或其他非数字字符。
- 日记编辑区：
  - 支持 Markdown 输入。
  - 旁边显示同内容的实时 Markdown 预览。
- 月加班总时长显示：
  - 按当前月份汇总所有已记录加班时长。
  - 显示为 `X 小时 Y 分钟`。

## 3. 日历状态规则

- 若某日期存在加班时长记录，则该日期显示红点。
- 若某日期存在日记内容，则该日期显示绿点。
- 若某日期为休息日，则日期文字或背景使用浅色样式区分。
- 若某日期为休息日且存在加班记录，则该日期数字颜色变红，用于强调休息日加班。
- 同一日期可同时拥有加班、日记和休息日状态。

## 4. 数据保存

- 所有加班记录、日记内容、工作日规则都必须本地缓存。
- 应用下次启动后可以恢复：
  - 最近一次选择的日期。
  - 已保存的加班记录。
  - 已保存的日记。
  - 已保存的工作日配置。

## 5. 工作日配置

用户可以在设置页配置工作日规则，支持两种模式：

### 5.1 按周计算

用于固定周规律或大小周/大中小周等周期排班。

配置项：

- 工作日周期变化次数。
  - 默认值为 `1`。
  - 含义：每 `N` 周为一个周期。
  - 例如：
    - `1`：每周规律固定不变。
    - `2`：两周循环一次。
    - `3`：三周循环一次。
- 当前选中日期对应的周期周序号。
  - 用户需要填写当前选中日期属于周期内的第几周。
  - 该值不能大于周期次数。
- 周期配置列表。
  - 列表按周期中的每一周展开。
  - 每周可分别配置星期一到星期日是工作日还是休息日。
  - 默认方案为周六、周日双休。

效果：

- 日历根据周期周序号和日期位置，推导每个日期的工作/休息状态。

### 5.2 按天计算

用于不按周固定轮转的排班，例如上五休三。

配置项：

- 工作几天。
- 休息几天。
- 当前选中日期是工作周期中的第几天。

效果：

- 以用户选择的当前日期作为锚点，向前后铺设工作/休息周期。
- 日历根据 `工作 N 天 + 休息 M 天` 的模式推导每日状态。

### 5.3 模式切换

- 设置页提供单选控件切换：
  - 按周算
  - 按天算
- 切换后仅显示对应模式的配置项。

## 6. UI 结构建议

### 6.1 主界面

- 左侧：月历控件。
- 右侧：当日记录面板。
- 顶部或侧边：进入设置页入口。

### 6.2 设置页

- 显示当前选择日期。
- 提供工作日模式切换。
- 提供对应模式的配置表单。
- 提供保存按钮。

## 7. 输入校验

### 7.1 时间输入

- 小时和分钟输入框仅允许数字。
- 输入时实时拦截非数字字符。
- 粘贴内容也必须做校验过滤。

### 7.2 配置校验

- 按周模式下：
  - 周期次数必须大于等于 1。
  - 当前周序号必须在 `1 ~ 周期次数` 之间。
- 按天模式下：
  - 工作天数必须大于等于 1。
  - 休息天数必须大于等于 0。
  - 当前锚点天数必须符合定义范围。

## 8. 建议的数据对象

- `DailyRecord`
  - `Date`
  - `OvertimeHours`
  - `OvertimeMinutes`
  - `DiaryMarkdown`
- `WorkScheduleConfig`
  - `Mode`：按周 / 按天
  - `AnchorDate`
  - `WeekCycleCount`
  - `CurrentCycleWeekIndex`
  - `WeeklyPattern`
  - `WorkDays`
  - `RestDays`
  - `AnchorWorkDayIndex`
- `MonthlySummary`
  - `Month`
  - `TotalOvertimeMinutes`

## 9. 非功能需求

- 本地持久化应稳定可靠。
- 启动时应快速恢复界面状态。
- 日历渲染应能清晰展示状态，不影响基本可读性。
- Markdown 预览应尽量实时同步输入内容。
- 所有 XAML 布局优先使用 `Margin` 控制间距，不使用空的 `Grid` 行/列来充当间隔。

## 10. 验收标准

- 打开应用后默认定位到今天。
- 可选择任意日期并编辑加班与日记。
- 小时和分钟输入框只能输入数字。
- 日历上能正确显示红点、绿点和休息日样式。
- 月加班总时长显示正确。
- 关闭并重新打开应用后，已保存数据仍然存在。
- 设置页支持按周/按天两种工作日规则配置，并能影响日历展示。

## 11. 待确认项

- Markdown 预览采用什么控件或库实现。
- 本地缓存格式采用 JSON、SQLite 还是其他方案。
- 日历是使用系统 `Calendar` 控件定制，还是自定义月视图。
- 加班时长是否允许为空、以及为空时是否视为未记录。

## 12. WPF 页面结构

### 12.1 主窗口

- `MainWindow`
  - 承载整个应用布局。
  - 建议采用左右分栏布局。
  - 包含：
    - 月历区域
    - 当日记录区域
    - 设置入口

### 12.2 月历区域

- 显示当前月份。
- 支持点击日期切换选中项。
- 每个日期单元显示：
  - 日期数字
  - 红点：加班记录
  - 绿点：日记记录
  - 休息日浅色样式
  - 休息日加班时数字红色高亮

### 12.3 当日记录区域

- 时间录入区
  - 小时输入框
  - 分钟输入框
- 日记区
  - Markdown 编辑输入
  - Markdown 预览面板
- 汇总区
  - 当前月份加班总时长

### 12.4 设置页面

- `SettingsPage` 或 `SettingsView`
  - 工作日模式切换
  - 按周配置区
  - 按天配置区
  - 当前锚点日期显示
  - 保存按钮

### 12.5 建议的 MVVM 划分

- `MainViewModel`
  - 当前月份
  - 选中日期
  - 日历单元集合
  - 当日记录入口
- `DayRecordViewModel`
  - 小时、分钟、日记内容
  - 保存和加载当前日期记录
- `SettingsViewModel`
  - 工作日模式
  - 周期参数
  - 锚点日期和当前周序号
- `CalendarDayViewModel`
  - 日期
  - 是否选中
  - 是否休息日
  - 是否有加班
  - 是否有日记
  - 是否休息日加班高亮

## 13. 数据模型

### 13.1 日常记录

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

### 13.2 工作日配置

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

### 13.3 本地持久化根对象

```csharp
class AppDataStore
{
    public WorkScheduleConfig WorkScheduleConfig { get; set; } = new();
    public List<DailyRecord> DailyRecords { get; set; } = new();
    public DateOnly? LastSelectedDate { get; set; }
}
```

## 14. 本地存储方案

### 14.1 推荐方案

- 使用本地 JSON 文件保存配置与记录。
- 适合当前需求，简单、可读、易调试。
- 文件可拆分为：
  - `settings.json`：工作日配置和最后选中日期
  - `records.json`：日记和加班记录

### 14.2 存储内容

- `settings.json`
  - 工作日模式
  - 按周/按天配置
  - 锚点日期
  - 当前选中日期
- `records.json`
  - 每天的加班时长
  - 每天的日记 Markdown
  - 最后修改时间

### 14.3 读写策略

- 启动时加载本地文件并恢复状态。
- 每次用户保存记录或修改设置后立即写回本地。
- 写入时建议先写临时文件，再替换正式文件，避免异常中断导致文件损坏。

### 14.4 路径建议

- 建议存放在用户配置目录下，例如：
  - `%AppData%\\OvertimeTimer\\`

### 14.5 扩展预留

- 如果后续需要更强查询能力，可从 JSON 平滑迁移到 SQLite。
- 当前阶段优先采用 JSON，降低实现复杂度。
