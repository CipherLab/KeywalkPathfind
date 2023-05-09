using Dapper;
using Dapper.Contrib.Extensions;
using LazyCache;
using Microsoft.Data.SqlClient;
using System.Diagnostics.Metrics;

namespace KeyWalkAnalyzer
{
    public class KeywalkRepo
    {
        private IAppCache _cache = new CachingService();
        private string _connectionString;

        public KeywalkRepo(string connectionString)
        {
            this._connectionString = connectionString;
        }

        public CommandActionsHeader GetCachedCommandActionsHeader(string commandHash)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                return _cache.GetOrAdd(commandHash, () => connection.QueryFirstOrDefault<CommandActionsHeader>("SELECT * FROM [dbo].[CommandActionsHeaders] WHERE CommandHash = @CommandHash", new { CommandHash = commandHash }));
            }
        }

        public FileProcessed GetCachedFileProcessed(string fileName)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Read the FileProcessed entry from the database
                var existingFile = connection.QueryFirstOrDefault<FileProcessed>("SELECT * FROM [dbo].[FileProcesseds] WHERE FileName = @FileName", new { FileName = fileName });

                // Cache the entry (using LazyCache or another caching library)
                return existingFile;
            }
        }

        private HashSet<string> savedHashes = new HashSet<string>();

        public async Task SaveToDatabaseAsync(string command, int count)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    if (savedHashes.Count <= 0)
                    {
                        IEnumerable<string> result = await connection.QueryAsync<string>("SELECT CommandHash FROM [dbo].[CommandActionsHeaders]");
                        savedHashes = new HashSet<string>(result);
                    }
                    connection.Open();

                    // Start a transaction
                    //using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            CommandActionsHeader existingHeader = new CommandActionsHeader();
                            if (savedHashes.TryGetValue(command, out var hash))
                            {
                                // If a record exists, increment the occurrences count and update the record
                                existingHeader = connection.QueryFirstOrDefault<CommandActionsHeader>("SELECT * FROM [dbo].[CommandActionsHeaders] WHERE CommandHash = @CommandHash", new { CommandHash = hash });
                                existingHeader.ObservedCount += count;
                                connection.Update(existingHeader);
                            }
                            else
                            {
                                // If no record exists, create a new record with the hash and occurrences count
                                var newHeader = new CommandActionsHeader
                                {
                                    CommandHash = command,
                                    ObservedCount = count,
                                };

                                var headerId = await connection.InsertAsync<CommandActionsHeader>(newHeader);
                                existingHeader = newHeader;
                                existingHeader.CommandActionsHeaderId = headerId;
                                //transaction.Commit();
                                savedHashes.Add(command);
                            }

                            // Commit the transaction
                            // transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of an error
                            Console.WriteLine($"Error occurred while saving data: {ex.Message}");
                            //transaction.Rollback();
                        }
                        finally
                        {
                        }
                    }
                }
            }
            finally
            {
            }
        }

        public async Task SaveFileFlag(string fileName)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(_connectionString))
                {
                    connection.Open();

                    // Start a transaction
                    // using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            // Check if a record for the file exists in the FileProcessed table
                            var existingFile = GetCachedFileProcessed(fileName);

                            if (existingFile != null && existingFile.IsProcessed)
                            {
                                // If the file has already been processed, do nothing and return
                                return;
                            }

                            // If the file has not been processed yet, insert a new record into the FileProcessed table and set the IsProcessed flag
                            var fileProcessed = new FileProcessed
                            {
                                FileName = fileName,
                                IsProcessed = true
                            };

                            await connection.InsertAsync(fileProcessed);

                            // Commit the transaction
                            //transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            // Rollback the transaction in case of an error
                            Console.WriteLine($"Error occurred while saving data: {ex.Message}");
                            //transaction.Rollback();
                        }
                        finally
                        {
                        }
                    }
                }
            }
            finally
            {
            }
        }

        public async Task SaveToDatabaseAsync(IEnumerable<KeyValuePair<string, Tuple<string, int>>> commands)
        {
            foreach (var item in commands)
            {
                if (string.IsNullOrEmpty(item.Key) || item.Key.Length >= 550)
                    continue;

                await SaveToDatabaseAsync(
                    item.Value.Item1,
                    item.Value.Item2
                    );
            }
        }
    }
}