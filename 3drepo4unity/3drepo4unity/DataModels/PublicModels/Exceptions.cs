using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepoForUnity
{
    public class RepoModelLoadingException : Exception
    {
        public RepoModelLoadingException(string err) : base(err) { }
    }
}
