namespace MTCG_Server.Serialization
{
    public interface ISerializer
    {
        string Serialize(object data);
    }
}
