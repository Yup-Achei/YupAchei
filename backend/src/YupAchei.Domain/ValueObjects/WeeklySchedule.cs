using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record DailyHours
{
    public DayOfWeek DayOfWeek { get; }
    public TimeOnly OpenTime { get; }
    public TimeOnly CloseTime { get; }

    public bool ClosesNextDay => CloseTime <= OpenTime;

    public DailyHours(DayOfWeek dayOfWeek, TimeOnly openTime, TimeOnly closeTime)
    {
        if (openTime == closeTime)
        {
            throw new DomainException("O horário de abertura e fechamento não podem ser idênticos.");
        }

        DayOfWeek = dayOfWeek;
        OpenTime = openTime;
        CloseTime = closeTime;
    }
}

public sealed record WeeklySchedule
{
    private readonly IReadOnlyList<DailyHours> _entries;

    public IReadOnlyList<DailyHours> Entries => _entries;

    public WeeklySchedule(IEnumerable<DailyHours> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);
        _entries = entries.ToList().AsReadOnly();
    }

    public static WeeklySchedule Empty() => new([]);

    public bool IsOpenAt(DateTimeOffset timestamp)
    {
        if (_entries.Count == 0)
        {
            return false;
        }

        var currentDay = timestamp.DayOfWeek;
        var previousDay = currentDay == DayOfWeek.Sunday ? DayOfWeek.Saturday : currentDay - 1;
        var currentTime = TimeOnly.FromTimeSpan(timestamp.TimeOfDay);

        foreach (var entry in _entries)
        {
            if (entry.DayOfWeek == currentDay)
            {
                if (!entry.ClosesNextDay && currentTime >= entry.OpenTime && currentTime < entry.CloseTime)
                {
                    return true;
                }

                if (entry.ClosesNextDay && currentTime >= entry.OpenTime)
                {
                    return true;
                }
            }
            else if (entry.DayOfWeek == previousDay && entry.ClosesNextDay && currentTime < entry.CloseTime)
            {
                return true;
            }
        }

        return false;
    }
}
