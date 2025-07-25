using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Service
{
    public class DatabaseService : IDatabaseService
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _databasePath;
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        public DatabaseService()
        {
            _databasePath = Path.Combine(FileSystem.AppDataDirectory, "ContadorApp.db3");
        }

        public async Task<SQLiteAsyncConnection> GetConnectionAsync()
        {
            if (_database != null)
                return _database;

            await _semaphore.WaitAsync();
            try
            {
                if (_database == null)
                {
                    _database = new SQLiteAsyncConnection(_databasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);
                    await InitializeDatabaseAsync();
                }
            }
            finally
            {
                _semaphore.Release();
            }

            return _database;
        }

        public async Task InitializeDatabaseAsync()
        {
            var db = await GetConnectionAsync();

            // Criar tabelas do sistema legado
            await db.CreateTableAsync<Kit>();
            await db.CreateTableAsync<Preco>();
            await db.CreateTableAsync<Atividade>();
            await db.CreateTableAsync<CodigoBarras>();
            await db.CreateTableAsync<Produto>();

            // Criar tabelas novas (se necessário)
            // await db.CreateTableAsync<NovaTabela>();
        }

        #region Métodos Genéricos

        public async Task<int> InsertAsync<T>(T item) where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.InsertAsync(item);
        }

        public async Task<int> InsertAllAsync<T>(IEnumerable<T> items) where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.InsertAllAsync(items);
        }

        public async Task<int> UpdateAsync<T>(T item) where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.UpdateAsync(item);
        }

        public async Task<int> DeleteAsync<T>(T item) where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.DeleteAsync(item);
        }

        public async Task<int> DeleteAllAsync<T>() where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.DeleteAllAsync<T>();
        }

        public async Task<List<T>> GetAllAsync<T>() where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.Table<T>().ToListAsync();
        }

        public async Task<T> GetAsync<T>(object pk) where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.GetAsync<T>(pk);
        }

        public async Task<List<T>> QueryAsync<T>(string query, params object[] args) where T : new()
        {
            var db = await GetConnectionAsync();
            return await db.QueryAsync<T>(query, args);
        }

        #endregion

        public async Task CloseAsync()
        {
            if (_database != null)
            {
                await _database.CloseAsync();
                _database = null;
            }
        }

        public void Dispose()
        {
            CloseAsync().Wait();
            _semaphore?.Dispose();
        }
    }
}
