using System.IO;
using System.Threading.Tasks;

namespace NRest
{
    public interface IRequestBodyBuilder
    {
        void Build(Stream stream);

        Task BuildAsync(Stream stream);
    }
}
