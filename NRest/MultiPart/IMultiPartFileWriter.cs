using System.IO;
using System.Threading.Tasks;

namespace NRest.MultiPart
{
    internal interface IMultiPartFileWriter
    {
        void Write(Stream stream);

        Task WriteAsync(Stream stream);
    }
}
