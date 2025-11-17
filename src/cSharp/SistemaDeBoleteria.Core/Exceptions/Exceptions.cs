using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaDeBoleteria.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public BusinessException(string message) : base(message)
        {
        }
    }
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
    public class NoContentException : Exception
    {
        public NoContentException(string message) : base(message)
        {
        }
    }
    public class DataBaseException : Exception
    {
        public DataBaseException(string message) : base(message)
        {
            
        } 
    }
}