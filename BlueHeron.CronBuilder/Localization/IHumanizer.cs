
namespace BlueHeron.Cron.Localization;

/// <summary>
/// Interface definition for objects that convert <see cref="Expression"/> objects into human-readable, localized representations.
/// </summary>
public interface IHumanizer
{
    /// <summary>
    /// Converts the given <see cref="Expression"/> into a human-readable, localized representation.
    /// </summary>
    /// <param name="expression">The <see cref="Expression"/> to convert</param>
    /// <returns>A human-readable, localized representation of the <see cref="Expression"/></returns>
    string Humanize(Expression expression);
}