using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace NRest
{
    public interface IRequestBodyBuilder
    {
        void Build(Stream stream, Encoding encoding);

        Task BuildAsync(Stream stream, Encoding encoding);
    }
}
