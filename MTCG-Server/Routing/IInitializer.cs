namespace MTCG_Server.Routing
{
    public interface IInitializer<T> 
    {
        T Initialize();
    }
}
