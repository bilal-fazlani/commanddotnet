using System.Threading.Tasks;

namespace CommandDotNet
{
    public delegate Task<int> InterceptorExecutionDelegate();
}