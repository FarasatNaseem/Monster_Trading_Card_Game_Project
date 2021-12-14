namespace MTCG_Server.Parser
{
    public interface IParser<T>
    {
        T Parse(object data);
    }
}
