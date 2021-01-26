using System;
using System.Collections.Generic;
using MultiPartFileUpload.Compressions;

namespace MultiPartFileUpload.Managers
{
    public class CompressionManager
    {
        private readonly IDictionary<string, ICompression> _compressions;

        public bool CompressionExists(string name)
        {
            return _compressions.ContainsKey(name.ToLower());
        }
        public CompressionManager(IEnumerable<ICompression> compressions)
        {
            _compressions = new Dictionary<string, ICompression>();
            foreach (ICompression compression in compressions)
            {
                _compressions.Add(compression.Name.ToLower(), compression);
            }
        }

        public ICompression GetCompression(string compressionName)
        {
            if (!_compressions.ContainsKey(compressionName.ToLower()))
            {
                throw new Exception("No compression exists");
            }

            return _compressions[compressionName.ToLower()];
        }
    }
}
