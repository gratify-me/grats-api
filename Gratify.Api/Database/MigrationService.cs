using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.EntityFrameworkCore;

namespace Gratify.Api.Database
{
    public class MigrationService
    {
        private readonly TelemetryClient _telemetry;
        private readonly DatabaseSettings _settings;
        private readonly GratsDb _database;

        public MigrationService(TelemetryClient telemetry, GratsDb database, DatabaseSettings settings)
        {
            _telemetry = telemetry;
            _settings = settings;
            _database = database;
        }

        public async Task ApplyDatabaseMigrationsAsync()
        {
            if (!_settings.ApplyMigrations.Value)
            {
                _telemetry.TrackEvent("Skipping database migration", new Dictionary<string, string>
                {
                    { "ApplyMigrations", _settings.ApplyMigrations.Value.ToString() }
                });
                return;
            }

            var pendingMigrations = await _database.Database.GetPendingMigrationsAsync();
            if (!pendingMigrations.Any())
            {
                _telemetry.TrackEvent("Skipping database migration: No pending migrations");
                return;
            }

            _telemetry.TrackEvent("Pending migrations", new Dictionary<string, string>
            {
                { "PendingMigrations", string.Join(",", pendingMigrations.ToArray()) }
            });

            try
            {
                await _database.Database.MigrateAsync();
            }
            catch (Exception ex)
            {
                _telemetry.TrackEvent("Database migration failed");
                _telemetry.TrackException(ex);
                throw ex;
            }

            _telemetry.TrackEvent("Database migration complete");
        }
    }
}