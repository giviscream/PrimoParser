using Application.Analyzers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Analyzers
{
    public abstract class ChangesAnalyzer<T, T1> : IChangesAnalyzer<T, T1>
    {
        public abstract bool IsDifferent(T newVersion, T prevVersion);

        public abstract T1 GetChanges(T newVersion, T prevVersion);

        public string GetHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
    }
}
