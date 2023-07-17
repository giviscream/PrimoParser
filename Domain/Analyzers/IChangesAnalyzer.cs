
namespace Application.Analyzers
{
    public interface IChangesAnalyzer<T, T1>
    {
        T1 GetChanges(T newVersion, T prevVersion);

        bool IsDifferent(T newVersion, T prevVersion);

    }

    
}
