using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStoreManager.Services
{
    public interface ITypeHelperService
    {
        bool TypeHasProps<T>(string fields);
    }
}
