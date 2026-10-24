using YupAchei.Domain.Exceptions;

namespace YupAchei.Domain.ValueObjects;

public sealed record DailyHours
{
    public DayOfWeek DiaSemana { get; }
    public TimeOnly HorarioAbertura { get; }
    public TimeOnly HorarioFechamento { get; }

    /// <summary>
    /// Indica se o fechamento ocorre após a meia-noite (no dia seguinte).
    /// </summary>
    public bool FechaNoDiaSeguinte => HorarioFechamento <= HorarioAbertura;

    public DailyHours(DayOfWeek diaSemana, TimeOnly horarioAbertura, TimeOnly horarioFechamento)
    {
        if (horarioAbertura == horarioFechamento)
        {
            throw new DomainException("O horário de abertura e fechamento não podem ser idênticos.");
        }

        DiaSemana = diaSemana;
        HorarioAbertura = horarioAbertura;
        HorarioFechamento = horarioFechamento;
    }
}

public sealed record WeeklySchedule
{
    private readonly IReadOnlyList<DailyHours> _horarios;

    public IReadOnlyList<DailyHours> Horarios => _horarios;

    public WeeklySchedule(IEnumerable<DailyHours> horarios)
    {
        ArgumentNullException.ThrowIfNull(horarios);
        _horarios = horarios.ToList().AsReadOnly();
    }

    public static WeeklySchedule Vazia() => new([]);

    public bool EstaAbertoEm(DateTimeOffset dataHora)
    {
        if (_horarios.Count == 0)
        {
            return false;
        }

        var diaAtual = dataHora.DayOfWeek;
        var diaAnterior = diaAtual == DayOfWeek.Sunday ? DayOfWeek.Saturday : diaAtual - 1;
        var horaAtual = TimeOnly.FromTimeSpan(dataHora.TimeOfDay);

        foreach (var horario in _horarios)
        {
            if (horario.DiaSemana == diaAtual)
            {
                if (!horario.FechaNoDiaSeguinte && horaAtual >= horario.HorarioAbertura && horaAtual < horario.HorarioFechamento)
                {
                    return true;
                }

                if (horario.FechaNoDiaSeguinte && horaAtual >= horario.HorarioAbertura)
                {
                    return true;
                }
            }
            else if (horario.DiaSemana == diaAnterior && horario.FechaNoDiaSeguinte && horaAtual < horario.HorarioFechamento)
            {
                return true;
            }
        }

        return false;
    }
}
