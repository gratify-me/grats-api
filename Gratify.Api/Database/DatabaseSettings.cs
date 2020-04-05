using System;

namespace Gratify.Api.Database
{
    public class DatabaseSettings
    {
        public bool? UseInMemory { get; set; }

        public bool? ApplyMigrations { get; set; }

        public string ConnectionString { get; set; }

        public void EnsureValid()
        {
            if (!UseInMemory.HasValue)
            {
                throw new ArgumentNullException($"{nameof(UseInMemory)} must be either true or false");
            }
            else if (!ApplyMigrations.HasValue)
            {
                throw new ArgumentNullException($"{nameof(ApplyMigrations)} must be either true or false");
            }
            else if (UseInMemory.Value && ApplyMigrations.Value)
            {
                throw new ArgumentNullException($"Cannot {nameof(ApplyMigrations)} when {nameof(UseInMemory)} is true");
            }
        }
    }
}
