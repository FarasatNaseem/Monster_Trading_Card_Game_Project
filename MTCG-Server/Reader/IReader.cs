namespace MTCG_Server.Reader
{
    public interface IReader<T>
    {
        T Read();
    }
}