using Eklee.Azure.Functions.Http.Example.Models;

namespace Eklee.Azure.Functions.Http.Example
{
    public interface IDtoDomain
    {
        DtoResponse DoWork();
    }
}
